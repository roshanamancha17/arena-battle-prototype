using UnityEngine;

public class Base : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;   // drag prefab here in Inspector
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        // ✅ Find the world canvas if it exists
        Canvas worldCanvas = FindObjectOfType<Canvas>();

        // ✅ Only spawn the bar if we actually have a prefab
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 6f,
                Quaternion.identity
            );

            // ✅ Make sure it’s under the canvas so it becomes visible
            if (worldCanvas != null)
                hb.transform.SetParent(worldCanvas.transform, false);

            healthBar = hb.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.target = transform;
                healthBar.SetHealth(currentHealth, maxHealth);
            }
        }
        else
        {
            Debug.LogWarning($"{name}: No healthBarPrefab assigned in Base.cs!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            Destroy(gameObject);
    }
}
