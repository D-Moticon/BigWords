using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public Sprite sprite;
    public float basicSpriteScaleFactor = 1f;
    public GameObject enemyBody;
    public int coinsToDrop = 1;
    [SerializeReference] public List<EnemyAction> enemyActions;

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
            enemy.simpleSpriteRenderer.transform.localScale = new Vector3(-basicSpriteScaleFactor, basicSpriteScaleFactor, 1f);
            enemy.healthBar.transform.localPosition = new Vector2(0f, enemy.simpleSpriteRenderer.bounds.size.y / 2f * 65f);
        }

        else
        {
            enemy.simpleSpriteRenderer.sprite = null;
            enemy.simpleSpriteRenderer.enabled = false;
        }

        enemy.enemyActions = new List<EnemyAction>(enemyActions);
        enemy.coinsToDrop = coinsToDrop;

        return enemy;
    }
}
