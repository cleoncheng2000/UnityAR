using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using System.Linq;
using Unity.Multiplayer.Center.Common;
using Vuforia;
using TMPro;


public class ProjectileManager : MonoBehaviour
{

    public GameObject targetPrefab;
    public GameObject bombPrefab;
    public GameObject bulletPrefab;
    public GameObject badmintonPrefab;
    public GameObject golfPrefab;
    public GameObject boxingPrefab;
    public GameObject fencingPrefab;
    public GameObject shieldPrefab;
    public GameObject snowParticlePrefab;
    public Transform MainCamera;

    // Spawn points for different projectiles
    public Transform bombSpawnPoint;
    public Transform bulletSpawnPoint;

    public Transform badmintonSpawnPoint;

    public Transform golfSpawnPoint;

    public Transform boxingSpawnPoint;

    public Transform fencingSpawnPoint;

    private GameObject shieldInstance; // Store the spawned shield reference

    // UI buttons for different projectiles
    public Button bombButton;
    public Button bulletButton;
    public Button badmintonButton;
    public Button golfButton;
    public Button boxingButton;
    public Button fencingButton;
    public Button shieldButton;
    public Button selfShieldButton;

    public TMP_Text snowParticleCountText; // Text to display the snow particle count

    public AudioManagerVuforia audioManager; // Reference to the audio manager
    public ObserverBehaviour observerBehaviour; // Reference to the Vuforia observer behaviour
    public List<Vector3> SnowParticlePositions = new List<Vector3>(); // Store the positions of snow particles
    // Dictionary to map anchors to snow particle effects
    private int snowParticleCount = 0;
    private bool currentStatus = false; // Track the current status of the target

    void Start()
    {
        bombButton.onClick.AddListener(FireBomb);
        bulletButton.onClick.AddListener(FireBullet);
        badmintonButton.onClick.AddListener(FireBadminton);
        golfButton.onClick.AddListener(FireGolf);
        boxingButton.onClick.AddListener(FireBoxing);
        fencingButton.onClick.AddListener(FireFencing);
        shieldButton.onClick.AddListener(() => ToggleShield(5));
        observerBehaviour.OnTargetStatusChanged += onTargetStatusChanged; // Subscribe to the target status change event
    }

    void onTargetStatusChanged(ObserverBehaviour observerBehaviour, TargetStatus targetStatus)
    {
        if (targetStatus.Status == Status.TRACKED)
        {
            currentStatus = true;
        }
        else
        {
            currentStatus = false;
        }
    }

    public void FireBomb()
    {
        GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, MainCamera.rotation);
        audioManager.PlayBombSound(); // Play the bomb sound
        if (currentStatus == true && IsTargetInView(observerBehaviour.transform))
        {
            bomb.GetComponent<ArcProjectile>().FireArcProjectile(observerBehaviour.transform, true, 5 * snowParticleCount);
            CreateSnowParticleEffect(observerBehaviour.transform.position);
            SnowParticlePositions.Add(observerBehaviour.transform.position); // Store the position of the snow particle
        }
        else
        {
            // Calculate a position 3 meters in front of the camera
            Vector3 forwardPosition = MainCamera.position + MainCamera.forward * 3f;
            GameObject tempTarget = new GameObject("TempTarget");
            tempTarget.transform.position = forwardPosition;
            bomb.GetComponent<ArcProjectile>().FireArcProjectile(tempTarget.transform, false, 0);
            Destroy(tempTarget, 3f); // Clean up the temporary target after 3 seconds
        }
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(MainCamera.forward, Vector3.up));
        GunRecoil gunController = MainCamera.GetComponentInChildren<GunRecoil>();
        gunController.Shoot();
        audioManager.PlayGunSound(); // Play the gun sound
        if (observerBehaviour != null && IsTargetInView(observerBehaviour.transform))
        {

            bullet.GetComponent<StraightProjectiles>().FireStraightProjectile(observerBehaviour.transform, true, 5 * snowParticleCount);
        }
        else
        {
            FireAtTemporaryTarget(bullet, bulletSpawnPoint, false);
        }
    }

    public void FireBadminton()
    {
        GameObject shuttle = Instantiate(badmintonPrefab, badmintonSpawnPoint.position, MainCamera.rotation);
        audioManager.PlayBadmintonSound(); // Play the badminton sound
        if (observerBehaviour != null && IsTargetInView(observerBehaviour.transform))
        {
            shuttle.GetComponent<ArcProjectile>().FireArcProjectile(observerBehaviour.transform, true, 5 * snowParticleCount);
        }
        else
        {
            FireAtTemporaryTarget(shuttle, badmintonSpawnPoint, true);
        }
    }

    public void FireBoxing()
    {
        GameObject glove = Instantiate(boxingPrefab, boxingSpawnPoint.position, Quaternion.LookRotation(MainCamera.forward, Vector3.up));
        audioManager.PlayBoxingSound(); // Play the boxing sound
        if (observerBehaviour != null && IsTargetInView(observerBehaviour.transform))
        {
            glove.GetComponent<StraightProjectiles>().FireStraightProjectile(observerBehaviour.transform, true, 5 * snowParticleCount);
        }
        else
        {
            FireAtTemporaryTarget(glove, boxingSpawnPoint, false);
        }
    }

    public void FireGolf()
    {
        GameObject missile = Instantiate(golfPrefab, golfSpawnPoint.position, MainCamera.rotation);
        audioManager.PlayGolfSound(); // Play the golf sound
        if (observerBehaviour != null && IsTargetInView(observerBehaviour.transform))
        {
            missile.GetComponent<ArcProjectile>().FireArcProjectile(observerBehaviour.transform, true, 5 * snowParticleCount);
        }
        else
        {
            FireAtTemporaryTarget(missile, golfSpawnPoint, true);
        }
    }

    public void FireFencing()
    {
        GameObject bullet = Instantiate(fencingPrefab, fencingSpawnPoint.position, Quaternion.LookRotation(MainCamera.forward, Vector3.up));
        audioManager.PlayFencingSound(); // Play the fencing sound
        if (observerBehaviour != null && IsTargetInView(observerBehaviour.transform))
        {
            bullet.GetComponent<StraightProjectiles>().FireStraightProjectile(observerBehaviour.transform, true, 5 * snowParticleCount);
        }
        else
        {
            FireAtTemporaryTarget(bullet, fencingSpawnPoint, false);
        }
    }

    public void ToggleShield(int shieldHp)
    {
        if (shieldHp > 0)
        {
            if (observerBehaviour != null && IsTargetInView(observerBehaviour.transform))
            {
                if (shieldInstance == null)
                {
                    // Spawn the shield
                    shieldInstance = Instantiate(shieldPrefab, observerBehaviour.transform.position, observerBehaviour.transform.rotation);
                    shieldInstance.transform.parent = observerBehaviour.transform;
                    Debug.Log("Shield activated.");
                }
            }
        }
        else
        {
            // Despawn the shield if HP is 0 or less
            if (shieldInstance != null)
            {
                Destroy(shieldInstance);
                shieldInstance = null;
                Debug.Log("Shield deactivated due to 0 HP.");
            }
        }
    }

    public void Reload()
    {
        GunRecoil gunController = MainCamera.GetComponentInChildren<GunRecoil>();
        gunController.Reload();
        audioManager.PlayReloadSound(); // Play the reload sound

        if (snowParticleCount > 0)
        {
            SpawnsDamagePopups.Instance.DamageDone(5 * snowParticleCount, observerBehaviour.transform.position, false);
        }
    }

    public bool IsTargetInView(Transform target)
    {
        if (target == null)
        {
            return false;
        }
        Vector3 viewportPoint = MainCamera.GetComponent<Camera>().WorldToViewportPoint(target.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }

    private void CreateSnowParticleEffect(Vector3 position)
    {
        GameObject snowParticleInstance = Instantiate(snowParticlePrefab, position, Quaternion.identity);
        SnowParticleProjectile snowParticleProjectile = snowParticleInstance.GetComponent<SnowParticleProjectile>();
        // if (snowParticleProjectile != null)
        // {
        //     snowParticleProjectile.target = trackedTarget.transform;
        // }
    }

    private void FireAtTemporaryTarget(GameObject missile, Transform spawnPoint, bool isArcProjectile)
    {
        Vector3 forwardPosition = MainCamera.position + MainCamera.forward * 3f;
        GameObject tempTarget = new GameObject("TempTarget");
        tempTarget.transform.position = forwardPosition;

        if (isArcProjectile)
        {
            missile.GetComponent<ArcProjectile>().FireArcProjectile(tempTarget.transform, false, 0);
        }
        else
        {
            missile.GetComponent<StraightProjectiles>().FireStraightProjectile(tempTarget.transform, false, 0);
        }

        Destroy(tempTarget, 3f); // Clean up the temporary target after 3 seconds
    }

    // Method to increment the counter
    public void IncrementSnowParticleCount()
    {
        snowParticleCount++;
        snowParticleCountText.text = snowParticleCount.ToString();
        Debug.Log($"Snow particles in range: {snowParticleCount}");
    }

    // Method to decrement the counter
    public void DecrementSnowParticleCount()
    {
        snowParticleCount = Mathf.Max(0, snowParticleCount - 1); // Ensure it doesn't go below 0
        snowParticleCountText.text = snowParticleCount.ToString();
        Debug.Log($"Snow particles in range: {snowParticleCount}");
    }

    // Method to get the current count
    public int GetSnowParticleCount()
    {
        // Get the position of the observerBehaviour
        Vector3 observerPosition = observerBehaviour.transform.position;

        // Initialize a counter for snow particles within range
        int count = 0;

        // Iterate through the list of snow particle positions
        foreach (Vector3 snowParticlePosition in SnowParticlePositions)
        {
            // Calculate the distance between the observer and the snow particle
            float distance = Vector3.Distance(observerPosition, snowParticlePosition);

            // Check if the distance is within 1 meter
            if (distance <= 1.0f)
            {
                count++;
            }
        }

        // Return the count of snow particles within range
        Debug.Log($"Snow particles within range: {count}");
        return count;
    }

    public void UpdateAction(string action, int p2ShieldHp)
    {
        ToggleShield(p2ShieldHp);
        switch (action)
        {
            case "bomb":
                FireBomb();
                break;
            case "gun":
                FireBullet();
                break;
            case "badminton":
                FireBadminton();
                break;
            case "golf":
                FireGolf();
                break;
            case "boxing":
                FireBoxing();
                break;
            case "fencing":
                FireFencing();
                break;
            case "reload":
                Reload();
                break;
            default:
                break;
        }
    }
}