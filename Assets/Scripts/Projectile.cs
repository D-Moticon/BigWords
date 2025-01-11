using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;

public class Projectile : MonoBehaviour
{
    public static IEnumerator FireProjectile(ProjectileSO projectileSO, EffectParams effectParams, float baseDamage, bool multiplyByCardCountMult = false, bool multiplyByCardPower = false)
    {
        Vector2 initialPos = effectParams.sourcePos;
        Vector2 finalPos = effectParams.targetPos;
        finalPos += Vector2.right * Random.Range(-.5f, .5f) + Vector2.up * Random.Range(-.5f, .5f);
        Actor target = effectParams.target;

        Vector2 dir = (finalPos - initialPos).normalized;
        float dist = (finalPos - initialPos).magnitude;

        float finalDamage = baseDamage;

        if (effectParams.overrideDamage)
        {
            finalDamage = effectParams.damageOverride;
        }

        if (multiplyByCardCountMult)
        {
            finalDamage *= effectParams.cardCountMult;
        }

        if (multiplyByCardPower)
        {
            if (effectParams.sourceCard != null)
            {
                finalDamage *= effectParams.sourceCard.GetPower();
            }
        }

        projectileSO.shotSFX.Play();
        if (projectileSO.muzzleVFX != null)
        {
            ParticleSystem vfx = ParticleSystem.Instantiate(projectileSO.muzzleVFX);
            vfx.transform.position = initialPos;
            vfx.transform.rotation = Helpers.Vector2ToRotation(dir);
        }

        Projectile p = Projectile.Instantiate(projectileSO.projectilePrefab, initialPos, Helpers.Vector2ToRotation(dir));

        float duration = dist / projectileSO.projectileSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            p.transform.position = Vector2.Lerp(initialPos, finalPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (projectileSO.impactVFX != null && target != null)
        {
            projectileSO.impactVFX.Spawn(finalPos);
        }

        if (projectileSO.impactSFX != null && target != null)
        {
            projectileSO.impactSFX.Play();
        }

        GameObject.Destroy(p.gameObject);

        EffectParams eParams = effectParams.Copy();
        eParams.overrideDamage = true;
        eParams.damageOverride = finalDamage;
        

        for (int i = 0; i < projectileSO.projectileEffects.Count; i++)
        {
            Task t = new Task(projectileSO.projectileEffects[i].GetEffectTask(eParams));

            while (t.Running)
            {
                yield return null;
            }
        }

    }
}
