using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float attackDamage = 10f;
    public float attackRange = 3f;
    public float attackRate = 1f;

    [Header("Troop Type")]
    public bool isArcher = false;

    [Header("Archer Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Team Settings")]
    public string enemyTroopTag = "EnemyTroop";
    public string enemyBaseTag = "EnemyBase";
    public string friendlyTag = "PlayerTroop";

    [Header("Targets")]
    public Transform targetBase; // assign in inspector

    private NavMeshAgent agent;
    private float nextAttackTime = 0f;
    private Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError($"{gameObject.name} missing NavMeshAgent!");
            return;
        }

        if (targetBase != null)
            agent.SetDestination(targetBase.position);
    }

    void Update()
    {
        if (agent == null) return;

        // Death
        if (health <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // If no current target → find one
        if (currentTarget == null)
        {
            FindNearestEnemy();
            if (currentTarget == null && targetBase != null)
                agent.SetDestination(targetBase.position);
        }

        // Handle combat/movement
        if (currentTarget != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance <= attackRange)
            {
                agent.isStopped = true;

                if (Time.time >= nextAttackTime)
                {
                    AttackTarget();
                    nextAttackTime = Time.time + 1f / attackRate;
                }

                // Face target
                Vector3 dir = currentTarget.position - transform.position;
                dir.y = 0;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.position);
            }
        }
    }

    void AttackTarget()
    {
        if (currentTarget == null) return;

        // Attack logic differs for Archer vs Swordsman
        if (isArcher && projectilePrefab != null && shootPoint != null)
        {
            GameObject arrow = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Projectile proj = arrow.GetComponent<Projectile>();
            if (proj != null)
                proj.Initialize(currentTarget, this);
        }
        else
        {
            // Melee
            Troop enemy = currentTarget.GetComponent<Troop>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }

            Base targetBaseObj = currentTarget.GetComponent<Base>();
            if (targetBaseObj != null)
            {
                targetBaseObj.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTroopTag);
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject e in enemies)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = e.transform;
            }
        }

        if (nearest != null)
        {
            currentTarget = nearest;
        }
        else
        {
            // No enemy troop? → aim for enemy base
            GameObject baseObj = GameObject.FindGameObjectWithTag(enemyBaseTag);
            if (baseObj != null)
                currentTarget = baseObj.transform;
        }
    }
}
