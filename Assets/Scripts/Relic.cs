using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

public class Relic : MonoBehaviour
{
    public RelicSO relicSO;
    public MMF_Player relicFeel;
    public SFXInfo relicSFX;
    public SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        GameManager.CardTriggeredEvent += CardTriggered;
        GameManager.AttackCompletedEvent += AttackCompleted;
        Actor.ActorDiedEvent += ActorDiedListener;
    }

    private void OnDisable()
    {
        GameManager.CardTriggeredEvent -= CardTriggered;
        GameManager.AttackCompletedEvent -= AttackCompleted;
        Actor.ActorDiedEvent -= ActorDiedListener;
    }

    public void PlayFeelAndSFX()
    {
        relicFeel.PlayFeedbacks();
        relicSFX.Play();
    }

    public virtual void DrawPhaseEntered() { }
    public virtual void DrawPhaseExited() { }
    public virtual void PlayPhaseEntered() { }
    public virtual void PlayPhaseExited() { }
    public virtual void AttackPhaseEntered(AttackInfo attackInfo) { }
    public virtual void CardTriggered(ref List<IEnumerator> tasksToPerform, Card c)
    {
        relicSO.CardTriggered(this, ref tasksToPerform, c);
    }

    public virtual void AttackCompleted(ref List<IEnumerator> tasksToPerform, AttackInfo attackInfo)
    {
        relicSO.AttackCompleted(this, ref tasksToPerform, attackInfo);
    }

    public virtual void ActorDiedListener(Actor dyingActor, ref List<IEnumerator> tasksToPerform)
    {
        relicSO.ActorDied(this, dyingActor, ref tasksToPerform);
    }

    public static Relic CreateRelicFromSO(RelicSO rso)
    {
        Relic r = Instantiate(Singleton.Instance.gameManager.relicPrefab);
        r.relicSO = rso;

        if (rso.relicSprite != null)
        {
            r.spriteRenderer.sprite = rso.relicSprite;
        }

        return r;
    }
}
