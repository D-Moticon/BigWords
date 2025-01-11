using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.UI;

public class Relic : MonoBehaviour, IBuyable
{
    public RelicSO relicSO;
    public MMF_Player relicFeel;
    public MMF_Player hoverFeel;
    public SFXInfo relicSFX;
    public Image relicImage;
    public BoxCollider2D bc2d;
    public delegate void RelicDelegate(Relic r);
    public static event RelicDelegate relicHoveredEvent;
    public static event RelicDelegate relicDeHoveredEvent;
    public static event RelicDelegate relicClickedEvent;
    public static event RelicDelegate relicAddedEvent;
    public float basePrice = 0f;
    public bool playerOwned = false;

    private void OnEnable()
    {
        //Subscription events are called in the Add Relic function so they only get called when owned by a player (not in shops)
    }

    public void AddRelic()
    {
        playerOwned = true;
        GameManager.CardTriggeredEvent += CardTriggered;
        GameManager.AttackCompletedEvent += AttackCompleted;
        GameManager.PostAttackDiscardCompletedEvent += PostAttackDiscardCompleted;
        Actor.ActorDiedEvent += ActorDiedListener;
        BuyableEvents.GetFinalPriceEvent += GetBuyablePriceListener;

        relicAddedEvent?.Invoke(this);
    }

    private void OnDisable()
    {
        GameManager.CardTriggeredEvent -= CardTriggered;
        GameManager.AttackCompletedEvent -= AttackCompleted;
        GameManager.PostAttackDiscardCompletedEvent -= PostAttackDiscardCompleted;
        Actor.ActorDiedEvent -= ActorDiedListener;
        BuyableEvents.GetFinalPriceEvent -= GetBuyablePriceListener;
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
    public virtual void AttackPhaseEntered(EffectParams effectParams) { }
    public virtual void CardTriggered(ref List<IEnumerator> tasksToPerform, Card c)
    {
        relicSO.CardTriggered(this, ref tasksToPerform, c);
    }

    public virtual void AttackCompleted(ref List<IEnumerator> tasksToPerform, EffectParams effectParams)
    {
        relicSO.AttackCompleted(this, ref tasksToPerform, effectParams);
    }

    public virtual void PostAttackDiscardCompleted(ref List<IEnumerator> tasksToPerform, EffectParams effectParams)
    {
        relicSO.PostAttackDiscardCompleted(this, ref tasksToPerform, effectParams);
    }

    public virtual void ActorDiedListener(Actor dyingActor, ref List<IEnumerator> tasksToPerform)
    {
        relicSO.ActorDied(this, dyingActor, ref tasksToPerform);
    }

    public virtual void GetBuyablePriceListener(IBuyable buyable, ref float price, float bPrice)
    {
        relicSO.GetBuyablePrice(this, buyable, ref price, bPrice);
    }

    public static Relic CreateRelicFromSO(RelicSO rso)
    {
        Relic r = Instantiate(Singleton.Instance.gameManager.relicPrefab);
        r.relicSO = rso;
        r.basePrice = rso.basePrice;

        if (rso.relicSprite != null)
        {
            r.relicImage.sprite = rso.relicSprite;
        }

        r.name = $"{rso.relicName} (Relic)";

        return r;
    }

    private void Update()
    {
        if (bc2d.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            if (Singleton.Instance.selectionHandler.currentlyHoveredRelic != this)
            {
                hoverFeel.PlayFeedbacks();
            }

            relicHoveredEvent?.Invoke(this);

            if (Input.GetMouseButtonDown(0))
            {
                RelicClicked();
            }

        }

        else
        {
            relicDeHoveredEvent?.Invoke(this);
        }
    }

    public float GetBasePrice()
    {
        return basePrice;
    }

    public void BuySuccessful()
    {
        AddRelic();
    }

    

    public void RelicClicked()
    {
        relicClickedEvent?.Invoke(this);
        BuyableEvents.BuyableClicked(this);
    }

    public float GetFinalPrice()
    {
        float finalPrice = basePrice;
        BuyableEvents.GetFinalPrice(this, ref finalPrice, basePrice);

        return finalPrice;
    }

    public Vector2 GetLocalPriceTagPosition()
    {
        return new Vector2(0f, 2f);
    }
}
