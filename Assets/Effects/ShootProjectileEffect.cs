using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShootProjectileEffect : Effect
{
    public ProjectileSO projectileSO;
    public float baseDamage = 1f;
    public bool multiplyByCardCountMult = true;
    public bool multiplyByCardPower = true;

    public override string GetEffectDescription()
    {
        return $"{projectileSO.GetProjectileDescription()}";
    }

    protected override IEnumerator RunEffect(EffectParams effectParams)
    {
        Task t = new Task(Projectile.FireProjectile(projectileSO, effectParams, baseDamage, multiplyByCardCountMult, multiplyByCardPower));
        while (t.Running)
        {
            yield return null;
        }
    }
}
