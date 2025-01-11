using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecycleRelic", menuName = "Relics/RecycleRelic")]
public class RecycleRelic : RelicSO
{
    public override void AttackCompleted(Relic relicInstance, ref List<IEnumerator> tasksToPerform, EffectParams effectParams)
    {
        base.AttackCompleted(relicInstance, ref tasksToPerform, effectParams);

        tasksToPerform.Add(Recycle(relicInstance, effectParams));
    }

    IEnumerator Recycle(Relic relicInstance, EffectParams effectParams)
    {
        yield return new WaitForSeconds(0.25f);
        relicInstance.PlayFeelAndSFX();

        for (int i = 0; i < effectParams.cardsOnRack.Count; i++)
        {
            effectParams.cardsOnRack[i].MoveCardToDeck(Singleton.Instance.gameManager.playerDeck, 1f, true);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.25f);
        Singleton.Instance.gameManager.playerDeck.SmartShuffle(Singleton.Instance.gameManager.smartShuffleFactor);
    }
}
