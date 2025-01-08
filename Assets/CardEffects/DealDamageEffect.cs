using UnityEngine;
using System.Collections;

[System.Serializable]
public class DealDamageEffect : CardEffect
{
    public float damage;

    public override IEnumerator CardActivatedEffect(AttackInfo attackInfo)
    {
        attackInfo.target.Damage(damage);

        if (feelPlayer != null)
        {
            feelPlayer.PlayFeedbacks();

            while (feelPlayer.IsPlaying)
            {
                yield return null;
            }
        }

        Debug.Log($"{attackInfo.source} used {owningCard.GetCardName()} to damage {attackInfo.target} for {damage}");
    }

    public override string GetEffectDescription()
    {
        return ($"+{damage} Damage");
    }
}
