using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FXSO", menuName = "Scriptable Objects/FXSO")]
public class FXSO : ScriptableObject
{
    [SerializeReference] public List<FXStep> fxSteps;

    public virtual IEnumerator PlayFXSteps(GameObject source = null, GameObject target = null)
    {
        for (int i = 0; i < fxSteps.Count; i++)
        {
            Task t = new Task(fxSteps[i].PlayFXStep(source, target));
            while (t.Running)
            {
                yield return null;
            }
        }

        for (int i = 0; i < fxSteps.Count; i++)
        {
            Task t = new Task(fxSteps[i].FXChainFinished(source, target));
            while (t.Running)
            {
                yield return null;
            }
        }
    }
}