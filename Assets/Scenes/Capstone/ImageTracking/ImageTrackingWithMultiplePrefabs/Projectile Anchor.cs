using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ProjectileAnchor : MonoBehaviour
{

    private Rigidbody rb;
    private bool targetHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // When the projectile collides with something...
    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)
        {
            return;
        }
        else {
            targetHit = true;
        }

        rb.isKinematic = true; 
        transform.SetParent(collision.transform);
    }
}
