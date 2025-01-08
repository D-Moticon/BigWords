using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyEncounterSO", menuName = "Scriptable Objects/EnemyEncounterSO")]
public class EnemyEncounterSO : ScriptableObject
{

    [System.Serializable]
    public class EnemyInfo
    {
        public EnemySO enemy;
        public Vector2 position;
        public float startingHP=10f;
    }

    public List<EnemyInfo> enemyInfos;
}
