using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackRate = 1f;

    [Header("Archer Settings")]
    public bool isArcher = false;
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Targets")]
    public Transform targetBase;

    private NavMeshAgent agent;
    private float nextAttackTime = 0f;
    private Troop currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Safety check
        if (agent == null)
        {
            Debug.LogError($"{gameObject.name} has no NavMeshAgent! Please add one.");
            return;
        }

        if (targetBase != null)
            agent.SetDestination(targetBase.position);
    }

    void Update()
    {
        if (agent == null) return;

        // Destroy when dead
        if (health <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Find a target if none exists
        if (currentTarget == null)
        {
            FindNearestEnemy();
        }
        else
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            // In range → attack
            if (distance <= attackRange)
            {
                agent.isStopped = true;

                // Attack on cooldown
                if (Time.time >= nextAttackTime)
                {
                    Attack(currentTarget);
                    nextAttackTime = Time.time + 1f / attackRate;
                }

                // Rotate to face enemy smoothly
                Vector3 lookPos = currentTarget.transform.position - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
            }
        }
    }

    void Attack(Troop enemy)
{
    if (enemy == null) return;

    if (isArcher && projectilePrefab != null && shootPoint != null)
    {
        GameObject arrow = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Projectile proj = arrow.GetComponent<Projectile>();
        if (proj != null)
            proj.Initialize(enemy.transform, this); // ✅ use Initialize instead of SetTarget
    }
    else
    {
        enemy.TakeDamage(attackDamage);
    }
}


    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void FindNearestEnemy()
    {
        Troop[] allTroops = FindObjectsOfType<Troop>();
        float minDistance = Mathf.Infinity;
        Troop nearestEnemy = null;

        foreach (Troop t in allTroops)
        {
            if (t == this) continue;
            if (t.tag == this.tag) continue;

            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestEnemy = t;
            }
        }

        if (nearestEnemy != null)
            currentTarget = nearestEnemy;
    }
}
