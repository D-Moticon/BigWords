using UnityEngine;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Collections;

public class FeelPoolManager : MonoBehaviour
{
    public int numberInstancesPerFeel = 5;
    public List<FeelSO> feelSOs;
    List<FeelPool> feelPools = new List<FeelPool>();

    private void Start()
    {
        for (int i = 0; i < feelSOs.Count; i++)
        {
            FeelPool fp = CreateFeelPool(feelSOs[i]);
            feelPools.Add(fp);
        }
    }

    public FeelPool CreateFeelPool(FeelSO feelSO)
    {
        GameObject pool = new GameObject(feelSO.name);
        FeelPool feelPool = pool.AddComponent<FeelPool>();
        pool.transform.SetParent(this.transform);
        feelPool.PopulatePool(feelSO, numberInstancesPerFeel);
        return feelPool;
    }

    public IEnumerator PlayFeelFromPoolTask(FeelSO feelSO, GameObject objectToFeel)
    {
        bool feelSOFound = false;
        for (int i = 0; i < feelPools.Count; i++)
        {
            if (feelPools[i].feelSO == feelSO)
            {
                feelSOFound = true;
                break;
            }
        }

        if (!feelSOFound)
        {
            feelPools.Add(CreateFeelPool(feelSO));
            yield return null; //one frame to allow list to populate
        }

        for (int i = 0; i < feelPools.Count; i++)
        {
            if (feelPools[i].feelSO == feelSO)
            {
                Task t = new Task(feelPools[i].PlayFeelFromPoolTask(objectToFeel));
                while (t.Running)
                {
                    yield return null;
                }
            }
        }
    }
}