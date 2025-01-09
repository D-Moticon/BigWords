using UnityEngine;

[CreateAssetMenu(menuName = "Pooled Object Data", fileName = "NewPooledObjectData")]
public class PooledObjectData : ScriptableObject
{
    [SerializeField] private GameObject pooledPrefab;
    [SerializeField] private float defaultLifetime = 2f;

    public GameObject PooledPrefab => pooledPrefab;
    public float DefaultLifetime => defaultLifetime;

    /// <summary>
    /// Spawns an object at the given position/rotation from the associated pool.
    /// </summary>
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        if (ObjectPoolManager.Instance == null)
        {
            Debug.LogError("No ObjectPoolManager present in the scene!");
            return null;
        }

        return ObjectPoolManager.Instance.SpawnObject(this, position, rotation);
    }

    /// <summary>
    /// Spawns an object at the origin with no rotation.
    /// </summary>
    public GameObject Spawn()
    {
        return Spawn(Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// Optionally, a helper to spawn at a position only (if you want rotation = identity).
    /// </summary>
    public GameObject Spawn(Vector3 position)
    {
        return Spawn(position, Quaternion.identity);
    }
}
