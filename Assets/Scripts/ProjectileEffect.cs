using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class ProjectileEffect
{
    public abstract IEnumerator ProjectileHitTask(Actor source, Actor target, Card c = null, AttackInfo attackInfo = null);
}
