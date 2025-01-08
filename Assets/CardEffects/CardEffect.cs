using UnityEngine;
using MoreMountains.Feedbacks;
using System.Collections;

[System.Serializable]
public abstract class CardEffect
{
    [HideInInspector]public Card owningCard;
    public MMF_Player feelPlayerPrefab;
    [HideInInspector]public MMF_Player feelPlayer;

    public abstract IEnumerator CardActivatedEffect(AttackInfo attackInfo);
    public abstract string GetEffectDescription();

    
    public virtual CardEffect CloneViaReflection()
    {
        return (CardEffect)this.MemberwiseClone();
    }

    public virtual CardEffect Clone()
    {
        // By default, just do a reflection or memberwise clone
        return CloneViaReflection();
    }
}
