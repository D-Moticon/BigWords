using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class DamagingProjectileEffect : CardEffect
{
    public Projectile projectilePrefab;
    public int numberProjectiles = 1;
    public ParticleSystem muzzleVFX;
    public SFXInfo shotSFX;
    public PooledObjectData impactVFX;
    public SFXInfo impactSFX;
    
    public float damage = 1f;
    public float projectileSpeed = 25f;

    public bool waitForProjectileImpact = true;
    [ShowIf("@waitForProjectileImpact == false")]
    public float shotToDamageDuration = 0.1f;

    public override IEnumerator CardActivatedEffect(AttackInfo attackInfo)
    {
        Vector2 initialPos = attackInfo.sourcePosition;
        Vector2 finalPos = attackInfo.targetPosition;

        Vector2 dir = (finalPos - initialPos).normalized;
        float dist = (finalPos - initialPos).magnitude;

        float finalDamage = attackInfo.cardCountMultiplier * damage * numberProjectiles;

        if (feelPlayer != null)
        {
            feelPlayer.PlayFeedbacks();
        }
        shotSFX.Play();

        if (muzzleVFX != null)
        {
            ParticleSystem vfx = ParticleSystem.Instantiate(muzzleVFX);
            vfx.transform.position = initialPos;
            vfx.transform.rotation = Helpers.Vector2ToRotation(dir);
        }

        Task projectileFlightTask = new Task(ProjectileFlight(initialPos, finalPos, dir, dist));

        if (waitForProjectileImpact)
        {
            while (projectileFlightTask.Running)
            {
                yield return null;
            }
        }

        else
        {
            yield return new WaitForSeconds(shotToDamageDuration);
        }

        if (attackInfo.target != null)
        {

            Task t = new Task(attackInfo.target.Damage(finalDamage));

            impactSFX.Play();
            if (impactVFX != null)
            {
                impactVFX.Spawn(finalPos);
            }

            while (t.Running)
            {
                yield return null;
            }
        }
    }

    IEnumerator ProjectileFlight(Vector2 initialPos, Vector2 finalPos, Vector2 dir, float dist)
    {
        Vector2[] finalPoses = new Vector2[numberProjectiles];
        Projectile[] ps = new Projectile[numberProjectiles];

        for (int i = 0; i < numberProjectiles; i++)
        {
            Projectile p = Projectile.Instantiate(projectilePrefab, initialPos, Helpers.Vector2ToRotation(dir));
            ps[i] = p;

            Vector2 fp = finalPos + Vector2.right * Random.Range(-.5f, .5f) + Vector2.up * Random.Range(-.5f, .5f);
            finalPoses[i] = fp;
        }

        float duration = dist / projectileSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            for (int i = 0; i < ps.Length; i++)
            {
                Projectile p = ps[i];
                p.transform.position = Vector2.Lerp(initialPos, finalPoses[i], t);
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < ps.Length; i++)
        {
            GameObject.Destroy(ps[i].gameObject);
        }
        
    }

    public override string GetEffectDescription()
    {
        return ($"+{damage*numberProjectiles} Damage");
    }
}
