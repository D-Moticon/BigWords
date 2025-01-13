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
    public FXSO shooterFX;

    public override string GetEffectDescription()
    {
        return $"{projectileSO.GetProjectileDescription()}";
    }

    protected override IEnumerator RunEffect(EffectParams effectParams)
    {
        if (shooterFX != null && effectParams.sourceCard != null)
        {
            Task shooterFXtask = new Task(shooterFX.PlayFXSteps(effectParams.sourceCard.gameObject));
            while (shooterFXtask.Running)
            {
                yield return null;
            }
        }

        Task t = new Task(Projectile.FireProjectile(projectileSO, effectParams, baseDamage, multiplyByCardCountMult, multiplyByCardPower));
        while (t.Running)
        {
            yield return null;
        }
    }
}
