using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public float lifeTime = 5f;

    private Transform target;
    private Troop shooterTroop;
    private bool hasHit = false;

    public void Initialize(Transform enemy, Troop shooter)
    {
        target = enemy;
        shooterTroop = shooter;

        // Face the target on spawn (aim near chest height)
        if (target != null)
        {
            transform.LookAt(target.position + Vector3.up * 1.2f);
        }

        // Auto-destroy after some time to avoid clutter
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (hasHit) return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move toward target each frame
        Vector3 targetPosition = target.position + Vector3.up * 1.2f;
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotate arrow to face travel direction
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        // Check if close enough to count as a hit
        if (Vector3.Distance(transform.position, targetPosition) < 0.4f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (hasHit) return;
        hasHit = true;

        if (target != null)
        {
            // Confirm it's an enemy before applying damage
            if (target.CompareTag("EnemyTroop"))
            {
                Troop enemyTroop = target.GetComponent<Troop>();
                if (enemyTroop != null && enemyTroop != shooterTroop)
                {
                    enemyTroop.TakeDamage(damage);
                }
            }
        }

        // (Optional) Add particle or sound here for hit feedback
        Destroy(gameObject);
    }

    // Optional physics trigger (for backup detection)
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("EnemyTroop"))
        {
            Troop enemy = other.GetComponent<Troop>();
            if (enemy != null && enemy != shooterTroop)
            {
                enemy.TakeDamage(damage);
            }

            hasHit = true;
            Destroy(gameObject);
        }
    }
}
