using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Transform target;
    private Troop shooter;
    private bool hasHit = false;

    public void Initialize(Transform enemy, Troop shooterTroop)
    {
        target = enemy;
        shooter = shooterTroop;

        if (target != null)
            transform.LookAt(target.position + Vector3.up * 1.2f);

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (hasHit) return;

        // ❌ If target or shooter has been destroyed → destroy projectile safely
        if (target == null || shooter == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = target.position + Vector3.up * 1.2f;
        Vector3 dir = (targetPos - transform.position).normalized;

        transform.position += dir * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);

        if (Vector3.Distance(transform.position, targetPos) < 0.4f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (hasHit) return;
        hasHit = true;

        if (target == null || shooter == null)
        {
            Destroy(gameObject);
            return;
        }

        // ✅ Safely check before accessing components
        Troop targetTroop = target.GetComponent<Troop>();
        if (targetTroop != null && targetTroop != shooter && target.tag != shooter.tag)
        {
            targetTroop.TakeDamage(damage);
        }

        Base baseObj = target.GetComponent<Base>();
        if (baseObj != null && target.tag != shooter.tag)
        {
            baseObj.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit || shooter == null) return;

        Troop t = other.GetComponent<Troop>();
        if (t != null && t != shooter && other.tag != shooter.tag)
        {
            t.TakeDamage(damage);
            hasHit = true;
            Destroy(gameObject);
        }

        Base b = other.GetComponent<Base>();
        if (b != null && other.tag != shooter.tag)
        {
            b.TakeDamage(damage);
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
