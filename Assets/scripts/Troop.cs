using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    public Transform targetBase;      // Assign enemy base from Spawner
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (targetBase != null)
        {
            agent.SetDestination(targetBase.position);
        }
    }
}
