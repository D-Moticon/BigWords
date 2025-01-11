using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SplittingProjectile", menuName = "Projectiles/SplittingProjectile")]
public class SplittingProjectile : ProjectileSO
{
    /*public ProjectileSO subProjectile;

    public enum SubProjectileDamageMode
    {
        constant,
        parentPercentage
    }

    public SubProjectileDamageMode subProjectileDamageMode;

    [ShowIf("subProjectileDamageMode", SubProjectileDamageMode.constant)]
    public float subProjectileDamage = 1f;
    [ShowIf("subProjectileDamageMode", SubProjectileDamageMode.parentPercentage)]
    public float subProjectileDamagePercentage = 0.5f;

    public override IEnumerator ProjectileHitTask(Actor source, Actor target, Card c = null, AttackInfo attackInfo = null)
    {
        if (target == null)
        {
            yield break;
        }

        Vector2 targetPos = target.transform.position;

        Task t = new Task(target.Damage(damage));

        while (t.Running)
        {
            yield return null;
        }

        if ((target!=null && target.isDying) || target == null)
        {
            List<Actor> subTargets = Singleton.Instance.gameManager.GetLivingEnemies();
            List<Task> subTasks = new List<Task>();

            float subDam = subProjectileDamage;

            foreach (Actor subTarget in subTargets)
            {
                switch (subProjectileDamageMode)
                {
                    case SubProjectileDamageMode.constant:
                        subDam = subProjectileDamage;
                        break;
                    case SubProjectileDamageMode.parentPercentage:
                        subDam = damage * subProjectileDamagePercentage;
                        break;
                }

                Task subT = new Task(Projectile.FireProjectile(subProjectile, targetPos, subTarget.transform.position, source, subTarget, subDam));
                subTasks.Add(subT);
            }

            for (int i = 0; i < subTasks.Count; i++)
            {
                while(subTasks[i].Running)
                {
                    yield return null;
                }
            }
        }

    }

    public override string GetProjectileDescription()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator ProjectileHitTask(Actor source, Actor target, Card c = null, AttackInfo attackInfo = null)
    {
        throw new System.NotImplementedException();
    }*/
}
