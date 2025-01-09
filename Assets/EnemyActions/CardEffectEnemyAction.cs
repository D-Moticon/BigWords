using UnityEngine;
using System.Collections;

[System.Serializable]
public class CardEffectEnemyAction : EnemyAction
{
    [SerializeReference]public CardEffect cardEffect;

    public override IEnumerator ActionTask(Actor sourceEnemy)
    {
        AttackInfo attackInfo = new AttackInfo();
        attackInfo.source = sourceEnemy;
        Actor target = null;

        switch (targetType)
        {
            case TargetType.targetPlayer:
                target = Singleton.Instance.gameManager.player;
                break;
            default: target = Singleton.Instance.gameManager.player;
                break;
        }

        attackInfo.target = target;
        attackInfo.cardCountMultiplier = 1f;
        attackInfo.sourcePosition = sourceEnemy.transform.position;
        attackInfo.targetPosition = target.transform.position;

        Task t = new Task(cardEffect.CardActivatedEffect(attackInfo));
        while (t.Running)
        {
            yield return null;
        }
    }
}
