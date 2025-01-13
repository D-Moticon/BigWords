using System.Collections;
using UnityEngine;

public class SFXStep : FXStep
{
    public SFXInfo sfxInfo;

    protected override IEnumerator PlayStep(GameObject source = null, GameObject target = null)
    {
        sfxInfo.Play();
        yield break;
    }
}
