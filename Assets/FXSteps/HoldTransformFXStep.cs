using System.Collections;
using UnityEngine;

public class HoldTransformFXStep : FXStep
{
    public FXTarget fxTarget;
    public float moveDuration = 0.2f;
    public Vector2 positionShift = Vector2.zero;
    public float rotationShift = 0;
    public float scaleMult = 1f;
    public AnimationCurve timeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    protected override IEnumerator PlayStep(GameObject source = null, GameObject target = null)
    {
        Task t = new Task(LerpTransform(source, target, true));
        while (t.Running)
        {
            yield return null;
        }
    }

    public IEnumerator LerpTransform(GameObject source = null, GameObject target = null, bool forward = true)
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

        Vector2 initialPos = go.transform.position;
        Quaternion initialRotation = go.transform.rotation;
        Vector2 initialScale = go.transform.localScale;

        Vector2 finalPos;
        float finalAngle;
        Quaternion finalRotation;
        Vector2 finalScale;

        if (forward)
        {
            finalPos = initialPos + positionShift;
            finalAngle = initialRotation.z + rotationShift;
            finalRotation = Helpers.AngleDegToRotation(finalAngle);
            finalScale = initialScale * scaleMult;
        }

        else
        {
            finalPos = initialPos - positionShift;
            finalAngle = initialRotation.z - rotationShift;
            finalRotation = Helpers.AngleDegToRotation(finalAngle);
            finalScale = initialScale / scaleMult;
        }


        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            float p = timeCurve.Evaluate(t);

            go.transform.position = Vector2.Lerp(initialPos, finalPos, p);
            go.transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, p);
            go.transform.localScale = Vector2.Lerp(initialScale, finalScale, p);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        go.transform.position = finalPos;
        go.transform.rotation = finalRotation;
        go.transform.localScale = finalScale;
    }

    public override IEnumerator FXChainFinished(GameObject source = null, GameObject target = null)
    {
        Task t = new Task(LerpTransform(source, target, false));
        while (t.Running)
        {
            yield return null;
        }
    }
}
