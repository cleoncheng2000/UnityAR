using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class ArcProjectile : MonoBehaviour
{
    public Transform target;  // The target (cube)
    public float throwSpeed = 5f;  // Speed at which the missile moves (horizontal speed)
    public float arcHeight = 3f;  // Height of the arc
    public int damage = 5;
    public GameObject snowParticlePrefab;
    private float startTime;
    private Vector3 startPosition;
    private bool isFired = false;
    private bool isHit = false;
    private int extraDamage;

    void Start()
    {
    }

    public void FireArcProjectile(Transform newTarget, bool newIsHit, int newExtraDamage)
    {
        target = newTarget;
        startPosition = transform.position;
        startTime = Time.time;  // Record the time at which the missile is fired
        isFired = true;
        isHit = newIsHit;
        extraDamage = newExtraDamage;
    }

    void Update()
    {
        if (isFired)
        {
            // Calculate time elapsed since missile was fired
            float timeElapsed = Time.time - startTime;
            // Move the missile horizontally toward the target
            float distance = Vector3.Distance(startPosition, target.position);
            float horizontalSpeed = throwSpeed;  // Constant speed in the horizontal direction
            float horizontalMovement = Mathf.Lerp(0, distance, timeElapsed * horizontalSpeed / distance);  // Interpolate towards the target

            // Calculate the vertical movement using a sine function to simulate the arc
            float verticalMovement = Mathf.Sin((timeElapsed * Mathf.PI) / (distance / horizontalSpeed)) * arcHeight;  // Sin wave for vertical arc (half-period)

            // Set the missile's new position
            Vector3 newPosition = Vector3.Lerp(startPosition, target.position, horizontalMovement / distance);
            newPosition.y += verticalMovement;  // Apply the arc height to the vertical position
            transform.position = newPosition;

            // Check if the missile has reached the target position
            if (horizontalMovement >= distance)
            {
                Destroy(gameObject);  // Destroy the missile when it reaches the target position
                // Damage(damage + extraDamage, isHit);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            Destroy(gameObject);  // Destroy the missile if it hits the target
            // Damage(damage, isHit);
        }
    }

    // void Damage(int damage, bool isHit) {
    //     if (isHit) {
    //         SpawnsDamagePopups.Instance.DamageDone(damage, target.transform.position, false);
    //     }
    //     else {
    //         SpawnsDamagePopups.Instance.DamageDone(0, target.transform.position, false);
    //     }
    // }
}