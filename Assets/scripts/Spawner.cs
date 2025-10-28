using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs & Points")]
    public GameObject swordsmanPrefab;
    public GameObject archerPrefab;
    public GameObject horsePrefab;
    public Transform spawnPoint;
    public Transform enemyBase;

    [Header("Spawn Effects & Sounds")]
    public GameObject spawnEffectPrefab;  // ✅ assign your spawn particle prefab
    public AudioClip spawnSound;          // ✅ assign spawn sound effect
    public float soundVolume = 0.8f;

    [Header("Energy System")]
    public int maxEnergy = 10;
    public int currentEnergy;
    public float energyRegenRate = 1f;
    public int swordsmanCost = 3;
    public int archerCost = 5;
    public int horseCost = 7;

    [Header("UI References (Optional)")]
    public Text energyText;
    public Button swordsmanButton;
    public Button archerButton;
    public Button horseButton;

    private float energyTimer;

    void Start()
    {
        StartCoroutine(InitializeSpawner());
    }

    IEnumerator InitializeSpawner()
    {
        yield return new WaitForSeconds(0.2f); // wait for scene setup
        currentEnergy = maxEnergy;
        UpdateUI();
    }

    void Update()
    {
        // Energy regeneration
        energyTimer += Time.deltaTime;
        if (energyTimer >= 1f / energyRegenRate)
        {
            energyTimer = 0f;
            if (currentEnergy < maxEnergy)
            {
                currentEnergy++;
                UpdateUI();
            }
        }

        // Optional keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Alpha1)) SpawnSwordsman();
        if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnArcher();
        if (Input.GetKeyDown(KeyCode.Alpha3)) SpawnHorse();

        UpdateButtonStates();
    }

    // === Public button methods ===
    public void SpawnSwordsman() => StartCoroutine(SafeSpawnTroop(swordsmanPrefab, swordsmanCost));
    public void SpawnArcher() => StartCoroutine(SafeSpawnTroop(archerPrefab, archerCost));
    public void SpawnHorse() => StartCoroutine(SafeSpawnTroop(horsePrefab, horseCost));

    IEnumerator SafeSpawnTroop(GameObject prefab, int cost)
    {
        if (currentEnergy < cost || prefab == null || spawnPoint == null)
            yield break;

        currentEnergy -= cost;
        UpdateUI();

        yield return new WaitForSeconds(0.05f); // small delay

        Vector3 spawnPos = spawnPoint.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
            spawnPos = hit.position;

        GameObject t = Instantiate(prefab, spawnPos, Quaternion.identity);

        Troop troopScript = t.GetComponent<Troop>();
        if (troopScript != null)
            troopScript.targetBase = enemyBase;

        yield return new WaitForEndOfFrame();

        NavMeshAgent agent = t.GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh && enemyBase != null)
            agent.SetDestination(enemyBase.position);

        // ✅ Play spawn particle and sound
        if (spawnEffectPrefab)
            Instantiate(spawnEffectPrefab, spawnPos, Quaternion.identity);

        if (spawnSound)
            AudioSource.PlayClipAtPoint(spawnSound, spawnPos, soundVolume);
    }

    void UpdateUI()
    {
        if (energyText != null)
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";
    }

    void UpdateButtonStates()
    {
        if (swordsmanButton != null)
            swordsmanButton.interactable = currentEnergy >= swordsmanCost;

        if (archerButton != null)
            archerButton.interactable = currentEnergy >= archerCost;

        if (horseButton != null)
            horseButton.interactable = currentEnergy >= horseCost;
    }
}
