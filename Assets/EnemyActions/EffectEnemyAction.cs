using UnityEngine;
using System.Collections;

[System.Serializable]
public class EffectEnemyAction : EnemyAction
{
    [SerializeReference]public Effect effect;

    public override IEnumerator ActionTask(Actor sourceEnemy)
    {
        
        EffectParams eParams = new EffectParams();

        switch (targetType)
        {
            case TargetType.targetPlayer:
                eParams.target = Singleton.Instance.gameManager.player;
                break;
            default:
                eParams.target = Singleton.Instance.gameManager.player;
                break;
        }

        eParams.cardCountMult = 1f;
        eParams.source = sourceEnemy;
        eParams.sourcePos = sourceEnemy.transform.position;
        eParams.targetPos = eParams.target.transform.position;

        Task t = new Task(effect.GetEffectTask(eParams));
        while (t.Running)
        {
            yield return null;
        }
    }
}
