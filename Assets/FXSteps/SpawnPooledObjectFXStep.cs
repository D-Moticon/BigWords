using System.Collections;
using UnityEngine;

public class SpawnPooledObjectFXStep : FXStep
{
    public PooledObjectData pooledObjectData;
    public Vector2 scale = Vector2.one;
    public FXTarget fxTarget;

    protected override IEnumerator PlayStep(GameObject source = null, GameObject target = null)
    {
        GameObject go = pooledObjectData.Spawn();
        go.transform.localScale = scale;

        switch (fxTarget)
        {
            case FXTarget.source:
                go.transform.position = source.transform.position;
                break;
            case FXTarget.target:
                go.transform.position = target.transform.position;
                break;
        }

        yield break;
    }
}
