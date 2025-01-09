using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExtraHitRelic", menuName = "Relics/ExtraHitRelic")]
public class ExtraHitRelic : RelicSO
{
    public float damage = 1f;
    public Projectile projectilePrefab;
    public SFXInfo projectileImpactSFX;
    public PooledObjectData projectileImpactVFX;
    public float projectileSpeed = 100f;

    public override void CardTriggered(Relic relicInstance, ref List<IEnumerator> tasksToPerform, Card c)
    {
        base.CardTriggered(relicInstance, ref tasksToPerform, c);

        Actor target = Singleton.Instance.gameManager.currentlyTargetedActor;
        if (target == null)
        {
            return;
        }

        //tasksToPerform.Add(DamageTarget(relicInstance));
        tasksToPerform.Add(Projectile.ProjectileFlightAndDamage(projectilePrefab, projectileSpeed, damage,
            relicInstance.transform.position, target.transform.position, target, projectileImpactVFX, projectileImpactSFX));
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
