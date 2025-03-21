using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RetargetRelic", menuName = "Relics/RetargetRelic")]
public class RetargetRelic : RelicSO
{
    public override void ActorDied(Relic relicInstance, Actor dyingActor, ref List<IEnumerator> tasksToPerform, EffectParams effectParams)
    {
        base.ActorDied(relicInstance, dyingActor, ref tasksToPerform, effectParams);

        if (dyingActor == Singleton.Instance.gameManager.currentlyTargetedActor || Singleton.Instance.gameManager.currentlyTargetedActor == null)
        {
            PlayFeelAndSFX();
            Singleton.Instance.gameManager.TargetFirstValidEnemy();
        }
    }
}
