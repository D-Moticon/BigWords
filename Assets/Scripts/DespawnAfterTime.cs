using UnityEngine;

public class DespawnAfterTime : MonoBehaviour
{
    public float lifeTime = 2f;
    float elapsedTime = 0f;

    private void OnEnable()
    {
        elapsedTime = 0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifeTime)
        {
            ObjectPoolManager.Instance.Despawn(gameObject);
        }
    }
}
