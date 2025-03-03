using System.Collections;
using UnityEngine;

public class SnowParticleProjectile : MonoBehaviour
{
    public int damage = 10;
    public Transform target;
    public float range = 1.0f; // Define the range within which the target should be to take damage
    public float damageCooldown = 3.0f; // Cooldown period in seconds

    private bool canDealDamage = true;

    void Update()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= range && canDealDamage)
            {
                StartCoroutine(DealDamageWithCooldown());
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

