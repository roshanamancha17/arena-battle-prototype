using UnityEngine;

public class CameraAutoSetup : MonoBehaviour
{
    public Transform playerBase;
    public Transform enemyBase;
    public float padding = 10f; // extra space around both bases

    private void Start()
    {
        if (playerBase == null || enemyBase == null)
        {
            Debug.LogError("Assign both playerBase and enemyBase in CameraAutoSetup!");
            return;
        }

        // Calculate midpoint
        Vector3 centerPoint = (playerBase.position + enemyBase.position) / 2f;

        // Calculate distance between bases
        float distance = Vector3.Distance(playerBase.position, enemyBase.position);

        // Position camera
        transform.position = new Vector3(centerPoint.x, distance * 0.6f, centerPoint.z - distance * 0.8f);
        transform.LookAt(centerPoint);
    }
}
