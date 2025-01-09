using UnityEngine;

[CreateAssetMenu(fileName = "HeroSO", menuName = "Scriptable Objects/HeroSO")]
public class HeroSO : ScriptableObject
{
    public Sprite basicSprite;
    public float basicSpriteScaleFactor = 1f;
    public GameObject heroBody;
    public float baseHP = 10f;

    public Actor CreateActorFromHeroSO(Actor actorPrefab)
    {
        Actor actor = Instantiate(actorPrefab);
        actor.simpleSpriteRenderer.sprite = basicSprite;
        actor.simpleSpriteRenderer.transform.localScale = new Vector3(basicSpriteScaleFactor, basicSpriteScaleFactor, 1f);
        actor.healthBar.transform.localPosition = new Vector2(0f, actor.simpleSpriteRenderer.bounds.size.y / 2f * 65f);

        return actor;
    }
}
