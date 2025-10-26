using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100f;
    public float attackDamage = 10f;
    public float attackRange = 4f;
    public float attackRate = 1f;
    public bool isArcher = false;
    public bool isHorse = false; // ✅ Horse: ignores enemies, charges base

    [Header("Archer Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    private HealthBar healthBar;
    private float maxHealth;

    [Header("Targets")]
    public Transform targetBase;

    private NavMeshAgent agent;
    private float nextAttackTime = 0f;
    private Transform currentTarget;

    private string enemyTroopTag;
    private string enemyBaseTag;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        maxHealth = health;

        // Setup team tags
        if (CompareTag("PlayerTroop"))
        {
            enemyTroopTag = "EnemyTroop";
            enemyBaseTag = "EnemyBase";
        }
        else
        {
            enemyTroopTag = "PlayerTroop";
            enemyBaseTag = "PlayerBase";
        }

        if (targetBase != null)
            agent.SetDestination(targetBase.position);

        // ✅ Spawn floating health bar
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            healthBar = hb.GetComponent<HealthBar>();
            healthBar.target = transform;
            healthBar.SetHealth(health, maxHealth);
        }
    }

    void Update()
    {
        if (health <= 0f)
        {
            if (healthBar != null)
                Destroy(healthBar.gameObject);
            Destroy(gameObject);
            return;
        }

        // ✅ Horse units always charge to base
        if (isHorse)
        {
            HandleHorseBehavior();
            return;
        }

        // 🗡️ Swordsman or Archer logic
        FindEnemyInRange();

        if (currentTarget == null)
        {
            if (targetBase != null)
            {
                agent.isStopped = false;
                agent.SetDestination(targetBase.position);
            }
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;

            if (Time.time >= nextAttackTime)
            {
                AttackTarget();
                nextAttackTime = Time.time + 1f / attackRate;
            }

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

    void HandleHorseBehavior()
    {
        if (targetBase == null) return;

        agent.isStopped = false;
        agent.SetDestination(targetBase.position);

        float distToBase = Vector3.Distance(transform.position, targetBase.position);
        if (distToBase <= attackRange)
        {
            agent.isStopped = true;
            if (Time.time >= nextAttackTime)
            {
                Base baseObj = targetBase.GetComponent<Base>();
                if (baseObj != null)
                    baseObj.TakeDamage(attackDamage);

                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void AttackTarget()
    {
        if (currentTarget == null) return;

        if (isArcher && projectilePrefab != null && shootPoint != null)
        {
            GameObject arrow = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Projectile proj = arrow.GetComponent<Projectile>();
            if (proj != null)
                proj.Initialize(currentTarget, this);
        }
        else
        {
            Troop enemy = currentTarget.GetComponent<Troop>();
            if (enemy != null)
                enemy.TakeDamage(attackDamage);

            Base targetBaseObj = currentTarget.GetComponent<Base>();
            if (targetBaseObj != null)
                targetBaseObj.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);

        if (health <= 0f)
        {
            if (healthBar != null)
                Destroy(healthBar.gameObject);
            Destroy(gameObject);
        }
    }

    void FindEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTroopTag);
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject e in enemies)
        {
            if (e == null) continue;
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist && d <= attackRange)
            {
                minDist = d;
                nearest = e.transform;
            }
        }

        if (nearest != null)
        {
            currentTarget = nearest;
        }
        else
        {
            GameObject baseObj = GameObject.FindGameObjectWithTag(enemyBaseTag);
            if (baseObj != null)
                currentTarget = baseObj.transform;
        }
    }
}
