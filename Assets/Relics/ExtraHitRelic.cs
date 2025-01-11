using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExtraHitRelic", menuName = "Relics/ExtraHitRelic")]
public class ExtraHitRelic : RelicSO
{
    public float damage = 1f;
    public ProjectileSO projectileSO;

    public override void CardTriggered(Relic relicInstance, ref List<IEnumerator> tasksToPerform, Card c)
    {
        base.CardTriggered(relicInstance, ref tasksToPerform, c);

        Actor target = Singleton.Instance.gameManager.currentlyTargetedActor;
        if (target == null)
        {
            return;
        }

        //tasksToPerform.Add(Projectile.FireProjectile(projectileSO, c.transform.position, target.transform.position, null, target, damage, 1));
        PlayFeelAndSFX();
    }

    public IEnumerator DamageTarget(Relic relicInstance)
    {
        relicInstance.PlayFeelAndSFX();

        yield return new WaitForSeconds(0.1f);

        Task t = new Task(Singleton.Instance.gameManager.currentlyTargetedActor.Damage(damage));
        while (t.Running)
        {
            yield return null;
        }
    }

    public override string GetRelicDescription()
    {
        string s = $"Whenever a card is activated, deal {damage} damage to current target";
        return s;
    }
}
