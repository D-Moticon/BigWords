using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RelicSO : ScriptableObject
{
    public string relicName;
    [TextArea]
    public string relicDescription;
    public Sprite relicSprite;
    public SFXInfo relicSFX;
    public virtual void PlayFeelAndSFX()
    {
        relicSFX.Play();
    }

    public virtual void CardTriggered(Relic relicInstance, ref List<IEnumerator> tasksToPerform, Card c) { }
    public virtual void AttackCompleted(Relic relicInstance, ref List<IEnumerator> tasksToPerform, AttackInfo attackInfo) { }

    public virtual string GetRelicName()
    {
        return relicName;
    }
    public virtual string GetRelicDescription()
    {
        return relicDescription;
    }
}
