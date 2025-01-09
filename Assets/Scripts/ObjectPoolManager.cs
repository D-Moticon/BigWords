using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public struct PoolSetup
    {
        public PooledObjectData pooledData; // The ScriptableObject with our real prefab
        public int initialSize;             // How many we want pre-spawned
    }

    [SerializeField] private List<PoolSetup> poolsToCreate;

    // Dictionary: which ObjectPool corresponds to which PooledObjectData
    private Dictionary<PooledObjectData, ObjectPool> poolDictionary
        = new Dictionary<PooledObjectData, ObjectPool>();

    // Dictionary for despawning: store which pool each spawned object came from
    private Dictionary<GameObject, ObjectPool> objectToPoolMap
        = new Dictionary<GameObject, ObjectPool>();

    private void Awake()
    {
        // Basic Singleton pattern (optional)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Build each pool at runtime
        foreach (var setup in poolsToCreate)
        {
            // Create a new GameObject in the scene to hold this pool
            GameObject poolGO = new GameObject(setup.pooledData.name + " Pool");
            poolGO.transform.SetParent(transform);

            // Add the ObjectPool component to it
            ObjectPool pool = poolGO.AddComponent<ObjectPool>();

            // Set the prefab and pool size
            pool.SetPrefab(setup.pooledData.PooledPrefab);
            pool.SetInitialSize(setup.initialSize);

            // Initialize the pool (instantiating initialSize objects)
            pool.InitializePool();

            // Store it in the dictionary
            poolDictionary.Add(setup.pooledData, pool);
        }
    }

    /// <summary>
    /// Spawn an object from the pool associated with the given data.
    /// </summary>
    public GameObject SpawnObject(PooledObjectData data, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.TryGetValue(data, out var pool))
        {
            Debug.LogError($"No pool found for {data.name}!");
            return null;
        }

        // Spawn from the correct pool
        GameObject spawnedObj = pool.Spawn(position, rotation);

        // Record which pool it came from, for later despawn
        objectToPoolMap[spawnedObj] = pool;

        return spawnedObj;
    }

    /// <summary>
    /// Despawn the object by returning it to whichever pool it came from.
    /// </summary>
    public void Despawn(GameObject obj)
    {
        if (!objectToPoolMap.TryGetValue(obj, out var pool))
        {
            Debug.LogWarning(
                $"Object {obj.name} was not spawned by ObjectPoolManager or was already despawned."
            );
            return;
        }

        objectToPoolMap.Remove(obj);
        pool.Despawn(obj);
    }
}
