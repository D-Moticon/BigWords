using System.Collections;
using UnityEngine;

public class DamageEffect : Effect
{
    public float baseDamage = 1f;
    public bool multiplyByCardCountMult = true;
    public bool multiplyByCardPower = true;

    protected override IEnumerator RunEffect(EffectParams effectParams)
    {
        if (effectParams.target == null)
        {
            yield break;
        }

        float finalDamage = baseDamage;

        if (effectParams.overrideDamage)
        {
            finalDamage = effectParams.damageOverride;
        }

        if (multiplyByCardCountMult)
        {
            finalDamage *= effectParams.cardCountMult;
        }

        if (multiplyByCardPower)
        {
            if (effectParams.sourceCard != null)
            {
                finalDamage *= effectParams.sourceCard.GetPower();
            }
        }

        Task t = new Task(effectParams.target.Damage(finalDamage, effectParams));
        while (t.Running)
        {
            yield return null;
        }
    }

    public override string GetEffectDescription()
    {
        return ($"Deal {baseDamage} x Card Power");
    }
}
