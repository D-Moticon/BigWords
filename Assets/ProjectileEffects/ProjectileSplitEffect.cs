using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class ProjectileSplitEffect : Effect
{
    public ProjectileSO projectileSO;
    public float baseDamage = 1f;
    public bool multiplyByCardPower = true;
    public bool multiplyByCardCountMult = true;
    public bool onlySplitIfTargetKilled = false;

    protected override IEnumerator RunEffect(EffectParams effectParams)
    {
        if (effectParams.allActorsOnTargetTeam == null)
        {
            yield break;
        }

        if (onlySplitIfTargetKilled && !effectParams.targetKilled)
        {
            yield break;
        }

        if (effectParams.target == null && !effectParams.targetKilled)
        {
            yield break;
        }

        List<Task> tasks = new List<Task>();

        for (int i = 0; i < effectParams.allActorsOnTargetTeam.Count; i++)
        {
            //Don't split to self
            if (effectParams.allActorsOnTargetTeam[i] == effectParams.target)
            {
                continue;
            }

            EffectParams newParams = effectParams.Copy();
            newParams.sourcePos = effectParams.targetPos;
            newParams.target = effectParams.allActorsOnTargetTeam[i];
            newParams.targetPos = newParams.target.transform.position;
            newParams.overrideDamage = true;
            newParams.damageOverride = baseDamage;

            Task t = new Task(Projectile.FireProjectile(projectileSO, newParams, baseDamage, multiplyByCardCountMult, multiplyByCardPower));
            tasks.Add(t);
        }

        foreach (Task t in tasks)
        {
            while (t.Running)
            {
                yield return null;
            }
        }
    }

    public override string GetEffectDescription()
    {
        return "sssss";
    }
}
