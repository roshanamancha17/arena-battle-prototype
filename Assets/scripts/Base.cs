using UnityEngine;

public class Base : MonoBehaviour
{
    public float health = 1000f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Debug.Log(gameObject.name + " destroyed!");
            Destroy(gameObject);
        }
    }
}
