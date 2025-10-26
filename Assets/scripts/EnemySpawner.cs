using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemySwordsmanPrefab;
    public GameObject enemyArcherPrefab;
    public GameObject enemyHorsePrefab;
    public Transform spawnPoint;
    public Transform playerBase;

    [Header("Energy System")]
    public int maxEnergy = 10;
    public int currentEnergy;
    public float energyRegenRate = 1f;
    public int swordsmanCost = 3;
    public int archerCost = 5;
    public int horseCost = 7;
    public float spawnDecisionInterval = 5f;

    private float spawnTimer;
    private float energyTimer;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        // Energy refill
        energyTimer += Time.deltaTime;
        if (energyTimer >= 1f / energyRegenRate)
        {
            energyTimer = 0f;
            if (currentEnergy < maxEnergy)
                currentEnergy++;
        }

        // Decide to spawn periodically
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnDecisionInterval)
        {
            spawnTimer = 0f;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        int choice = Random.Range(0, 3); // 0 = swordsman, 1 = archer, 2 = horse
        GameObject prefab;
        int cost;

        if (choice == 0)
        {
            prefab = enemySwordsmanPrefab;
            cost = swordsmanCost;
        }
        else if (choice == 1)
        {
            prefab = enemyArcherPrefab;
            cost = archerCost;
        }
        else
        {
            prefab = enemyHorsePrefab;
            cost = horseCost;
        }

        if (currentEnergy >= cost)
        {
            currentEnergy -= cost;
            GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            Troop t = enemy.GetComponent<Troop>();
            enemy.tag = "EnemyTroop";
            if (t != null)
                t.targetBase = playerBase;
        }
    }
}
