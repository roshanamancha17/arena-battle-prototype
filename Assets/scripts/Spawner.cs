using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Troop Prefabs")]
    public GameObject swordsmanPrefab;
    public GameObject archerPrefab;
    public GameObject horsePrefab;

    [Header("Spawn & Enemy")]
    public Transform spawnPoint;      // Where troops spawn
    public Transform enemyBase;       // Enemy base

    [Header("Energy Settings")]
    public int energy = 10;           // Current energy
    public int swordsmanCost = 3;
    public int archerCost = 5;
    public int horseCost = 7;

    void Update()
    {
        // Mobile touch input: tap anywhere spawns Swordsman (optional)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SpawnSwordsman();
        }

        // Editor keyboard input for testing
        if (Input.GetKeyDown(KeyCode.Alpha1)) SpawnSwordsman();
        if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnArcher();
        if (Input.GetKeyDown(KeyCode.Alpha3)) SpawnHorse();
    }

    public void SpawnSwordsman() => SpawnTroop(swordsmanPrefab, swordsmanCost);
    public void SpawnArcher() => SpawnTroop(archerPrefab, archerCost);
    public void SpawnHorse() => SpawnTroop(horsePrefab, horseCost);

    void SpawnTroop(GameObject prefab, int cost)
    {
        if (energy >= cost)
        {
            energy -= cost;
            GameObject t = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            Troop troopScript = t.GetComponent<Troop>();
            if (troopScript != null)
            {
                troopScript.targetBase = enemyBase;
            }
        }
        else
        {
            Debug.Log("Not enough energy!");
        }
    }
}
