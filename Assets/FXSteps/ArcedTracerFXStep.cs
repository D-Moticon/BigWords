using System.Collections;
using UnityEngine;

public class ArcedTracerFXStep : FXStep
{
    public PooledObjectData tracerPooledObject;
    public float speed = 50f;
    public AnimationCurve verticalCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    public float verticalDistance = 0f;

    protected override IEnumerator PlayStep(GameObject source = null, GameObject target = null)
    {
        if (source == null || target == null)
        {
            yield break;
        }

        GameObject go = tracerPooledObject.Spawn();

        float duration = (source.transform.position - target.transform.position).magnitude / speed;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            go.transform.position = Vector2.Lerp(source.transform.position, target.transform.position, t);

            Vector2 verticalAdd = verticalCurve.Evaluate(t) * Vector2.up * verticalDistance;
            go.transform.position += (Vector3)verticalAdd;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        go.transform.position = target.transform.position;

        ObjectPoolManager.Instance.Despawn(go);
    }

}
