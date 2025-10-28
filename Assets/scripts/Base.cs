using UnityEngine;

public class Base : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 500f;
    private float currentHealth;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        // Find the world canvas (tagged "WorldCanvas")
        GameObject canvasObj = GameObject.FindGameObjectWithTag("WorldCanvas");
        Canvas worldCanvas = canvasObj != null ? canvasObj.GetComponent<Canvas>() : null;

        if (worldCanvas == null)
            Debug.LogWarning("⚠️ No Canvas with tag 'WorldCanvas' found!");

        // Spawn the health bar
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 6f,
                Quaternion.identity
            );

            if (worldCanvas != null)
                hb.transform.SetParent(worldCanvas.transform, false);

            healthBar = hb.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.target = transform;
                healthBar.SetHealth(currentHealth, maxHealth);
                healthBar.SetColorByTag(gameObject.tag);
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
        {
            // Trigger win/lose state
            LevelManager manager = FindObjectOfType<LevelManager>();
            if (manager != null)
            {
                if (CompareTag("PlayerBase"))
                    manager.ShowLose();
                else if (CompareTag("EnemyBase"))
                    manager.ShowWin();
            }

            // Destroy the base
            Destroy(gameObject);
        }
    }
}
