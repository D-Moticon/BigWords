using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public abstract class FXStep
{
    public bool waitForStepToFinish = true;
    public float postDelay = 0f;

    //These are called from other scripts
    public IEnumerator PlayFXStep(GameObject source = null, GameObject target = null)
    {
        Task t = new Task(PlayStep(source, target));
        if (waitForStepToFinish)
        {
            while (t.Running)
            {
                yield return null;
            }
        }

        if (postDelay > 0.0001f)
        {
            yield return new WaitForSeconds(postDelay);
        }
    }


    //These are the IEnumerators we make in child classes
    protected abstract IEnumerator PlayStep(GameObject source = null, GameObject target = null);

    public virtual IEnumerator FXChainFinished(GameObject source = null, GameObject target = null)
    {
        yield break;
    }
}
