using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

public class FeelPool : MonoBehaviour
{
    public FeelSO feelSO;
    public List<MMF_Player> instantiatedFeels = new List<MMF_Player>();
    public int currentIndex = 0;

    public IEnumerator PlayFeelFromPoolTask(GameObject go)
    {
        currentIndex++;
        if (currentIndex > instantiatedFeels.Count - 1)
        {
            currentIndex = 0;
        }

        MMF_Player mmfp = instantiatedFeels[currentIndex];

        //Stop the feel if it is already playing
        if (mmfp.IsPlaying)
        {
            mmfp.StopFeedbacks();
        }

        mmfp.transform.SetParent(go.transform);
        mmfp.Initialization(go);

        instantiatedFeels[currentIndex].PlayFeedbacks();
        while (instantiatedFeels[currentIndex].IsPlaying)
        {
            yield return null;
        }
    }

    public void PopulatePool(FeelSO fSO, int quantity)
    {
        feelSO = fSO;

        for (int i = 0; i < quantity; i++)
        {
            MMF_Player mmfPlayer = Instantiate(feelSO.feelPrefab, this.transform);
            instantiatedFeels.Add(mmfPlayer);
        }
    }
}