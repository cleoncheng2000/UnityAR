using UnityEngine;

public class StraightProjectiles : MonoBehaviour
{
    public Transform target;  // Reference to the target (cube prefab)
    public float speed = 5f;  // Speed at which the projectile moves
    public int damage = 10;  // Damage dealt by the projectile
    private bool isFired = false;
    private bool isHit = false;
    private int extraDamage = 0;

    public void FireStraightProjectile(Transform newTarget, bool newIsHit, int newExtraDamage)
    {
        target = newTarget;
        isFired = true;
        isHit = newIsHit;
        extraDamage = newExtraDamage;
    }

    void Update()
    {
        if (isFired && target != null)
        {
            // Move the projectile towards the target
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Check if the projectile has reached the target
            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                Destroy(gameObject);  // Destroy the projectile
                Damage(damage + extraDamage, isHit);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            Destroy(gameObject);  // Destroy the projectile on collision with the target
            Damage(5, isHit);
        }
    }

    void Damage(int damage, bool isHit)
    {
        if (isHit)
        {
            SpawnsDamagePopups.Instance.DamageDone(damage, target.transform.position, false);
        }
        else
        {
            SpawnsDamagePopups.Instance.DamageDone(0, target.transform.position, false);
        }
    }
}
