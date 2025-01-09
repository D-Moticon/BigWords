using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 10;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    /// <summary>
    /// Called once by the manager after this pool is created, 
    /// to pre-instantiate the initial batch of objects.
    /// </summary>
    public void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            var newObj = Instantiate(prefab, transform);
            newObj.SetActive(false);
            poolQueue.Enqueue(newObj);
        }
    }

    /// <summary>
    /// Spawns an object from the pool at the specified position/rotation.
    /// </summary>
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            // If no inactive objects remain, create a new one
            obj = Instantiate(prefab, transform);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Returns an object back to the pool.
    /// </summary>
    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }

    /// <summary>
    /// Allows the manager to set the prefab reference at runtime.
    /// </summary>
    public void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
    }

    /// <summary>
    /// Allows the manager to set the initial pool size at runtime.
    /// </summary>
    public void SetInitialSize(int size)
    {
        initialPoolSize = size;
    }
}
