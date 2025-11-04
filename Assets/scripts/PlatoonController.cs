using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Spawns and controls a small platoon (formation) of troops.
/// One leader (index 0) uses NavMeshAgent destination to move the group.
/// Other members follow offsets & can break formation to attack.
/// </summary>
public class PlatoonController : MonoBehaviour
{
    [Header("Platoon Settings")]
    public GameObject troopPrefab;          // Prefab (Troop + NavMeshAgent)
    public int count = 4;                   // Number of soldiers
    public float spacing = 1.4f;            // Distance between members
    public float detectRange = 6f;          // Detection range per soldier

    [Header("References")]
    public Transform spawnPoint;
    public Transform enemyBase;             // Enemy base target

    private List<GameObject> members = new List<GameObject>();
    private List<Vector3> offsets = new List<Vector3>();
    private NavMeshAgent leaderAgent;
    private Transform leaderTransform;

    /// ✅ **Button-callable version (no parameters!)**
    public void SpawnPlatoonButton()
    {
        if (spawnPoint == null || enemyBase == null)
        {
            Debug.LogError("Assign spawnPoint & enemyBase in Inspector.");
            return;
        }

        SpawnPlatoon(spawnPoint, enemyBase, count, spacing);
    }

    /// ✅ Actual spawn function
    public void SpawnPlatoon(Transform spawn, Transform targetBase, int num, float gap)
    {

        spawnPoint = spawn;
        enemyBase = targetBase;
        count = Mathf.Max(1, num);
        spacing = Mathf.Max(0.5f, gap);

        CalculateFormationOffsets(count, spacing);

        for (int i = 0; i < count; i++)
        {
            Vector3 worldPos = spawnPoint.position + spawnPoint.TransformDirection(offsets[i]);
            GameObject go = Instantiate(troopPrefab, worldPos, spawnPoint.rotation);

            // ✅ Set team tag
            go.tag = this.CompareTag("PlayerPlatoonSpawner") ? "PlayerTroop" : "EnemyTroop";

            // ✅ Ensure NavMeshAgent exists
            NavMeshAgent a = go.GetComponent<NavMeshAgent>();
            if (a == null)
                a = go.AddComponent<NavMeshAgent>();

            // ✅ Add PlatoonMember to follow formation
            PlatoonMember pm = go.AddComponent<PlatoonMember>();
            pm.Setup(this, i, offsets[i], detectRange, enemyBase);

            members.Add(go);

            if (i == 0)
            {
                leaderAgent = a;
                leaderTransform = go.transform;
            }
        }

        if (leaderAgent != null && enemyBase != null)
        {
            leaderAgent.SetDestination(enemyBase.position);
        }
    }

    /// ✅ Calculate grid formation (square-ish)
    private void CalculateFormationOffsets(int num, float spacing)
    {
        offsets.Clear();

        int cols = Mathf.CeilToInt(Mathf.Sqrt(num));
        int rows = Mathf.CeilToInt((float)num / cols);

        int idx = 0;
        float startX = -(cols - 1) * 0.5f * spacing;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (idx >= num) break;
                float x = startX + c * spacing;
                float z = -(r * spacing);
                offsets.Add(new Vector3(x, 0f, z));
                idx++;
            }
        }
    }

    void Update()
    {
        if (leaderAgent != null && enemyBase != null)
        {
            if (!leaderAgent.pathPending && leaderAgent.remainingDistance < 0.5f)
                leaderAgent.isStopped = true;
        }
    }

    /// ✅ Gives world position where member should stand
    public Vector3 GetWorldOffsetPosition(int index)
    {
        if (leaderTransform == null)
            return spawnPoint.position + offsets[index];

        return leaderTransform.TransformPoint(offsets[index]);
    }

    public Transform GetEnemyBase()
    {
        return enemyBase;
    }

    public void ClearPlatoon()
    {
        foreach (var m in members)
            if (m != null)
                Destroy(m);

        members.Clear();
        offsets.Clear();
        leaderAgent = null;
        leaderTransform = null;
    }

    void OnDestroy()
    {
        // ClearPlatoon();
    }
}
