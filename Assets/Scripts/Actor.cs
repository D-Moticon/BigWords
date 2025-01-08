using UnityEngine;

public class Actor : MonoBehaviour, IDamagable
{
    public float health = 10f;
    public float maxHealth = 10f;

    public void Damage(float damage)
    {
        health -= damage;
    }
}
