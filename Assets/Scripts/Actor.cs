using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using System.Collections.Generic;

public class Actor : MonoBehaviour, IDamagable
{
    public float health = 10f;
    public float maxHealth = 10f;
    public Slider healthBar;
    public TMP_Text healthText;
    public Collider2D col;

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

    public delegate void ActorDeathDelegate(Actor dyingActor);
    public static ActorDeathDelegate ActorDiedEvent;

    public bool isDying = false;

    [FoldoutGroup("Enemy")]
    public List<EnemyAction> enemyActions;
    [FoldoutGroup("Enemy")]
    public int currentActionIndex = 0;

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

    public void Damage(float damage)
    {
        health -= damage;
        UpdateHealthIndicators();
        PlayStandardHitFeel();

        Singleton.Instance.uiManager.SpawnDamageFloater(this.transform.position, damage);

        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, this.transform.position, Quaternion.identity);
        }

        if (deathFeel != null)
        {
            deathFeel.PlayFeedbacks();
        }

        deathSFX.Play();
        isDying = true;
        ActorDiedEvent?.Invoke(this);
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
            mpb.SetFloat("_PixelOutlineFade", amount);
            sr.SetPropertyBlock(mpb);
        }
    }
}
