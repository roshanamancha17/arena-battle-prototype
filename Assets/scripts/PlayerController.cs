using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Troop Prefabs")]
    public GameObject swordsmanPrefab;
    public GameObject archerPrefab;

    [Header("Spawn & Target")]
    public Transform spawnPoint;
    public Transform enemyBase;

    [Header("Energy Settings")]
    public int energy = 10;
    public int swordsmanCost = 3;
    public int archerCost = 5;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnTroop(swordsmanPrefab, swordsmanCost);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnTroop(archerPrefab, archerCost);
    }

    void SpawnTroop(GameObject prefab, int cost)
    {
        if (energy < cost)
        {
            Debug.Log("Not enough energy!");
            return;
        }

        energy -= cost;

        GameObject troopObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Troop troop = troopObj.GetComponent<Troop>();

        if (troop != null && enemyBase != null)
        {
            // ✅ Assign the Transform directly (NavMeshAgent uses Transform)
            troop.targetBase = enemyBase;
        }
    }
}
