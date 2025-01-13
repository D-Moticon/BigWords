using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Collections;

public class Actor : MonoBehaviour
{
    public float health = 10f;
    public float maxHealth = 10f;
    public Slider healthBar;
    public TMP_Text healthText;
    public BoxCollider2D col;

    [FoldoutGroup("Feels")]
    public MMF_Player standardHitFeel;

    public SpriteRenderer simpleSpriteRenderer;
    SpriteRenderer[] spriteRenderers;

    [FoldoutGroup("Death")]
    public GameObject deathVFXPrefab;
    [FoldoutGroup("Death")]
    public SFXInfo deathSFX;
    [FoldoutGroup("Death")]
    public MMF_Player deathFeel;

    public delegate void ActorDeathDelegate(Actor dyingActor, ref List<IEnumerator> tasksToPerform, EffectParams effectParams);
    public static ActorDeathDelegate ActorDiedEvent;

    public bool isDying = false;

    [FoldoutGroup("Enemy")]
    public List<EnemyAction> enemyActions;
    [FoldoutGroup("Enemy")]
    public int currentActionIndex = 0;
    [FoldoutGroup("Enemy")]
    public int coinsToDrop = 0;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        UpdateHealthIndicators();
    }

    void Update()
    {
        if (col.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            Singleton.Instance.selectionHandler.ActorHovered(this);
        }

        else
        {
            Singleton.Instance.selectionHandler.ActorNotHovered(this);
        }
    }

    public IEnumerator Damage(float damage, EffectParams effectParams = null)
    {
        health -= damage;
        UpdateHealthIndicators();
        PlayStandardHitFeel();

        Singleton.Instance.uiManager.SpawnDamageFloater(this.transform.position, damage);

        if (health <= 0f)
        {
            if (effectParams.target == this)
            {
                effectParams.targetKilled = true;
            }

            Task dieTask = new Task(Die());
            while (dieTask.Running)
            {
                yield return null;
            }
        }

        yield break;
    }

    public IEnumerator Die(EffectParams effectParams = null)
    {
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, this.transform.position, Quaternion.identity);
        }

        if (deathFeel != null)
        {
            deathFeel.PlayFeedbacks();
        }

        if (coinsToDrop > 0)
        {
            for (int i = 0; i < coinsToDrop; i++)
            {
                GameObject go = Singleton.Instance.gameManager.coinPickup.Spawn(this.transform.position, Quaternion.identity);
                Pickup p = go.GetComponent<Pickup>();
                p.InitializePickup(this.transform.position, Singleton.Instance.gameManager.player);
            }
        }

        deathSFX.Play();
        isDying = true;

        List<IEnumerator> tasksToPerform = new List<IEnumerator>();
        ActorDiedEvent?.Invoke(this, ref tasksToPerform, effectParams);

        for (int j = 0; j < tasksToPerform.Count; j++)
        {
            Task t = new Task(tasksToPerform[j]);
            while (t.Running)
            {
                yield return null;
            }
        }
    }

    public void SetHealthAndMaxHealth(float h)
    {
        health = h;
        maxHealth = h;
        UpdateHealthIndicators();
    }


    void UpdateHealthIndicators()
    {
        healthBar.value = health / maxHealth;
        healthText.text = (Mathf.CeilToInt(health)).ToString();
    }

    public void PlayStandardHitFeel()
    {
        if (standardHitFeel != null)
        {
            standardHitFeel.PlayFeedbacks();
        }
    }

    public void SelectActor()
    {
        OutlineSpriteRenderers(1f);
    }

    public void DeSelectActor()
    {
        OutlineSpriteRenderers(0f);
    }

    public void OutlineSpriteRenderers(float amount)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat("_OuterOutlineFade", amount);
            sr.SetPropertyBlock(mpb);
        }
    }
}
