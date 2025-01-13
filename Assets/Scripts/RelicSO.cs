using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RelicSO : ScriptableObject, IBuyableSO
{
    public string relicName;
    [TextArea]
    public string relicDescription;
    public Sprite relicSprite;
    public SFXInfo relicSFX;
    public float basePrice = 10f;

    public virtual void PlayFeelAndSFX()
    {
        relicSFX.Play();
    }

    public virtual void CardTriggered(Relic relicInstance, ref List<IEnumerator> tasksToPerform, Card c) { }
    public virtual void AttackCompleted(Relic relicInstance, ref List<IEnumerator> tasksToPerform, EffectParams effectParams) { }

    public virtual void PostAttackDiscardCompleted(Relic relicInstance, ref List<IEnumerator> tasksToPerform, EffectParams effectParams) { }

    public virtual void ActorDied(Relic relicInstance, Actor dyingActor, ref List<IEnumerator> tasksToPerform, EffectParams effectParams) { }

    public virtual void GetBuyablePrice(Relic relicInstance, IBuyable buyable, ref float price, float bPrice) { }

    public virtual string GetRelicName()
    {
        return relicName;
    }
    public virtual string GetRelicDescription()
    {
        return relicDescription;
    }

}
