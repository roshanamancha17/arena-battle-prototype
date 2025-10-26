using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs & Points")]
    public GameObject swordsmanPrefab;
    public GameObject archerPrefab;
    public GameObject horsePrefab;
    public Transform spawnPoint;
    public Transform enemyBase;

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

        // Optional: keep keyboard control too
        if (Input.GetKeyDown(KeyCode.Alpha1)) SpawnSwordsman();
        if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnArcher();
        if (Input.GetKeyDown(KeyCode.Alpha3)) SpawnHorse();

        UpdateButtonStates();
    }

    // === Public button methods ===
    public void SpawnSwordsman() => SpawnTroop(swordsmanPrefab, swordsmanCost);
    public void SpawnArcher() => SpawnTroop(archerPrefab, archerCost);
    public void SpawnHorse() => SpawnTroop(horsePrefab, horseCost);

    void SpawnTroop(GameObject prefab, int cost)
    {
        if (currentEnergy < cost) return;

        currentEnergy -= cost;
        GameObject t = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        Troop troopScript = t.GetComponent<Troop>();
        if (troopScript != null)
            troopScript.targetBase = enemyBase;

        UpdateUI();
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
