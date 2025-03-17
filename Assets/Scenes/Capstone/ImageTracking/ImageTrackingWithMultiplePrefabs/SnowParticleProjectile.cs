using System.Collections;
using UnityEngine;

public class SnowParticleProjectile : MonoBehaviour
{
    public int damage = 5;
    public Transform target;
    public float range = 1.0f; // Define the range within which the target should be to take damage
    public float damageCooldown = 3.0f; // Cooldown period in seconds
    public bool isInRange = false;

    private bool canDealDamage = true;
    private ProjectileEventManager eventManager;

    void Start()
    {
        eventManager = FindFirstObjectByType<ProjectileEventManager>();
    }
    void Update()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            bool wasInRange = isInRange;
            isInRange = distance <= range;

            // Notify the manager if the range status changes
            if (isInRange && !wasInRange)
            {
                eventManager?.IncrementSnowParticleCount();
            }
            else if (!isInRange && wasInRange)
            {
                eventManager?.DecrementSnowParticleCount();
            }
        }
    }

    private IEnumerator DealDamageWithCooldown()
    {
        canDealDamage = false;
        Damage(damage, true);
        yield return new WaitForSeconds(damageCooldown);
        canDealDamage = true;
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

