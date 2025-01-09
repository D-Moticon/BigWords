using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public static IEnumerator ProjectileFlightAndDamage(Projectile projectilePrefab, float projectileSpeed, float damage,
        Vector2 initialPos, Vector2 finalPos, Actor target = null, PooledObjectData vfxPooledObject = null, SFXInfo impactSFX = null)
    {
        Vector2 dir = (finalPos - initialPos).normalized;
        float dist = (finalPos - initialPos).magnitude;

        Projectile p = Projectile.Instantiate(projectilePrefab, initialPos, Helpers.Vector2ToRotation(dir));
        float duration = dist / projectileSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            p.transform.position = Vector2.Lerp(initialPos, finalPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        p.transform.position = finalPos;

        if (target != null)
        {
            target.Damage(damage);
            if (vfxPooledObject != null)
            {
                vfxPooledObject.Spawn(finalPos);
            }
            if (impactSFX != null)
            {
                impactSFX.Play();
            }
        }

        GameObject.Destroy(p.gameObject);
    }
}
