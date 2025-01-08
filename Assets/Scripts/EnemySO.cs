using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public Sprite sprite;
    public GameObject enemyBody;

    public Actor CreateEnemyFromSO()
    {
        Actor enemy = Instantiate(Singleton.Instance.gameManager.actorPrefab);

        if (enemyBody != null)
        {
            GameObject enemyBodyGO = Instantiate(enemyBody, enemy.transform);
            enemyBody.transform.localPosition = Vector3.zero;
            enemyBody.transform.rotation = Quaternion.identity;
        }

        if (sprite != null)
        {
            enemy.simpleSpriteRenderer.sprite = sprite;
        }

        else
        {
            enemy.simpleSpriteRenderer.sprite = null;
            enemy.simpleSpriteRenderer.enabled = false;
        }

        return enemy;
    }
}
