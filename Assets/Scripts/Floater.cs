using UnityEngine;
using MoreMountains.Feedbacks;

public class Floater : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float distance = 2f;
    [SerializeField] private AnimationCurve timeCurve = AnimationCurve.Linear(0f,0f,1f,1f);
    [SerializeField] private MMF_Player feel;
    Vector2 initialPos;
    Vector2 finalPos;
    private float timer;

    private TMPro.TextMeshPro textMesh; // or other UI text


    private void Awake()
    {
        textMesh = GetComponentInChildren<TMPro.TextMeshPro>();
    }

    public void SetText(string text)
    {
        if (textMesh) textMesh.text = text;
    }

    private void OnEnable()
    {
        timer = 0f;
        initialPos = transform.position;
        finalPos = transform.position + Vector3.up * distance;
        if (feel != null)
        {
            feel.PlayFeedbacks();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float t = timer / lifetime;
        float p = timeCurve.Evaluate(t);
        transform.position = Vector2.Lerp(initialPos, finalPos, p);

        if (timer >= lifetime)
        {
            // Despawn
            ObjectPoolManager.Instance.Despawn(gameObject);
            // Or you might store reference to the right pool if you have multiple
        }
    }
}
