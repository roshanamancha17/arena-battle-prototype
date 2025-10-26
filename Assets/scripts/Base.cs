using UnityEngine;

public class Base : MonoBehaviour
{
    public float health = 500f;

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0f)
        {
            Debug.Log($"{gameObject.name} destroyed!");
            Destroy(gameObject);
        }
    }
}
