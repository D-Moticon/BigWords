using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProjectileSO", menuName = "Scriptable Objects/ProjectileSO")]
public class ProjectileSO : ScriptableObject
{
    public string projectileName;
    public string projectileDescription;    
    public Projectile projectilePrefab;
    public float projectileSpeed = 120f;
    public ParticleSystem muzzleVFX;
    public SFXInfo shotSFX;
    public PooledObjectData impactVFX;
    public SFXInfo impactSFX;

    [SerializeReference] public List<Effect> projectileEffects;

    public virtual string GetProjectileDescription()
    {
        if (string.IsNullOrEmpty(projectileDescription))
        {
            return projectileName;
        }

        return projectileDescription;
    }
}
