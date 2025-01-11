using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public abstract class Effect
{
    public Phase activationPhase;
    public virtual IEnumerator GetEffectTask(EffectParams effectParams)
    {
        // Check if the current phase matches the effect's activation phase
        if (activationPhase != Phase.any)
        {
            if (effectParams.phase != activationPhase)
            {
                // If not the correct phase, cancel the effect by ending the coroutine immediately
                yield break;
            }
        }

        // If phase matches, defer to the actual effect logic implemented by derived classes
        yield return RunEffect(effectParams);
    }

    protected abstract IEnumerator RunEffect(EffectParams effectParams);

    public abstract string GetEffectDescription();
}
