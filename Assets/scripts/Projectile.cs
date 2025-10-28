using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 5f;

    [Header("Impact Effects")]
    public GameObject impactEffectPrefab;  // ✅ assign ArrowImpact prefab
    public AudioClip hitSound;             // ✅ assign hit sound
    public float hitVolume = 0.8f;

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

        // ✅ Play hit particle and sound
        if (impactEffectPrefab)
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

        if (hitSound)
            AudioSource.PlayClipAtPoint(hitSound, transform.position, hitVolume);

        // Apply damage
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
        Base b = other.GetComponent<Base>();

        if ((t != null && t != shooter && other.tag != shooter.tag) ||
            (b != null && other.tag != shooter.tag))
        {
            // ✅ Play hit effects
            if (impactEffectPrefab)
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

            if (hitSound)
                AudioSource.PlayClipAtPoint(hitSound, transform.position, hitVolume);

            if (t != null)
                t.TakeDamage(damage);
            else if (b != null)
                b.TakeDamage(damage);

            hasHit = true;
            Destroy(gameObject);
        }
    }
}
