using UnityEngine;
using System.Collections;

[System.Serializable]
public class DealDamageEffect : CardEffect
{
    public float damage;
    public SFXInfo impactSFX;

    public override IEnumerator CardActivatedEffect(AttackInfo attackInfo)
    {
        float finalDamage = damage * attackInfo.cardCountMultiplier;

        attackInfo.target.Damage(finalDamage);

        if (feelPlayer != null)
        {
            feelPlayer.PlayFeedbacks();

            yield return new WaitForSeconds(0.02f);
        }

        impactSFX.Play();
        Debug.Log($"{attackInfo.source} used {owningCard.GetCardName()} to damage {attackInfo.target} for {finalDamage}");
    }

    public override string GetEffectDescription()
    {
        return ($"+{damage} Damage");
    }
}
