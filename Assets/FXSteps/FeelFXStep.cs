using UnityEngine;
using System.Collections;

[System.Serializable]
public class FeelFXStep : FXStep
{
    public FeelSO feelSO;
    public FXTarget fxTarget;

    protected override IEnumerator PlayStep(GameObject source = null, GameObject target = null)
    {
        GameObject go = source;
        switch (fxTarget)
        {
            case FXTarget.source:
                go = source;
                break;
            case FXTarget.target:
                go = target;
                break;
        }


        Task t = new Task(Singleton.Instance.feelPoolManager.PlayFeelFromPoolTask(feelSO, go));
        while (t.Running)
        {
            yield return null;
        }
    }

}