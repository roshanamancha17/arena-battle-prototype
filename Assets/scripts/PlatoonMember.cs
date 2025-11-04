using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class PlatoonMember : MonoBehaviour
{
    private PlatoonController controller;
    private int index;
    private Vector3 localOffset;
    private float detectRange;
    private Transform enemyBase;
    private NavMeshAgent agent;
    private Troop troop; // existing Troop script (health, attack logic)

    private Transform currentEnemyTarget;
    private float checkInterval = 0.25f;
    private float checkTimer = 0f;

    public void Setup(PlatoonController parent, int memberIndex, Vector3 offset, float detectRange, Transform baseTransform)
    {
        controller = parent;
        index = memberIndex;
        localOffset = offset;
        this.detectRange = detectRange;
        enemyBase = baseTransform;

        agent = GetComponent<NavMeshAgent>();
        troop = GetComponent<Troop>();

        // ensure troop is melee: disable archer flag for platoon
        if (troop != null) troop.isArcher = false;

        // initial destination to formation position
        Vector3 world = controller.GetWorldOffsetPosition(index);
        agent.SetDestination(world);
    }

    void Update()
    {
        if (agent == null) return;

        // If we currently have a valid enemy target (alive and within detection), pursue/attack it
        if (currentEnemyTarget != null)
        {
            if (currentEnemyTarget.Equals(null))
            {
                currentEnemyTarget = null;
            }
            else
            {
                float d = Vector3.Distance(transform.position, currentEnemyTarget.position);
                if (d <= troop.attackRange)
                {
                    agent.isStopped = true;

                    // Cooldown check using Troop API
                    if (Time.time >= troop.GetLastAttackTime() + (1f / troop.attackRate))
                    {
                        Troop enemyTroop = currentEnemyTarget.GetComponent<Troop>();
                        if (enemyTroop != null)
                        {
                            enemyTroop.TakeDamage(troop.attackDamage);
                            troop.SetLastAttackTime(Time.time);
                        }
                        else
                        {
                            Base b = currentEnemyTarget.GetComponent<Base>();
                            if (b != null)
                            {
                                b.TakeDamage(troop.attackDamage);
                                troop.SetLastAttackTime(Time.time);
                            }
                        }
                    }
                }
                else if (d <= detectRange * 1.5f) // chase if reasonably close
                {
                    agent.isStopped = false;
                    agent.SetDestination(currentEnemyTarget.position);
                }
                else
                {
                    // lost enemy
                    currentEnemyTarget = null;
                }

                return;
            }
        }

        // Periodically search for enemies within detectRange
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = checkInterval;
            FindClosestEnemyInRange();
        }

        // If no enemy, follow formation offset position
        if (currentEnemyTarget == null)
        {
            Vector3 targetPos = controller.GetWorldOffsetPosition(index);
            float dist = Vector3.Distance(transform.position, targetPos);

            // set destination only if far enough to save performance
            if (dist > 0.5f)
            {
                agent.isStopped = false;
                agent.SetDestination(targetPos);
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }

    private void FindClosestEnemyInRange()
    {
        if (controller == null || string.IsNullOrEmpty(troop.enemyTroopTag)) return;

        // get all enemies by tag (cheap for small numbers; optimize later if necessary)
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(troop.enemyTroopTag);
        float minD = detectRange;
        Transform nearest = null;
        foreach (GameObject e in enemies)
        {
            if (e == null) continue;
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minD)
            {
                minD = d;
                nearest = e.transform;
            }
        }

        if (nearest != null)
        {
            currentEnemyTarget = nearest;
        }
    }

    // Expose small APIs via Troop (we'll reference nextAttackTimeLocal)
}
