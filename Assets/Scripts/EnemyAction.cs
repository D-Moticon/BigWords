using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class EnemyAction
{
    public enum TargetType
    {
        targetPlayer
    }

    public TargetType targetType;

    public abstract IEnumerator ActionTask(Actor sourceEnemy);
}
