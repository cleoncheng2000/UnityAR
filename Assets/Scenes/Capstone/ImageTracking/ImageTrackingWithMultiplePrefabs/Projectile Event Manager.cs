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


public class ProjectileEventManager : MonoBehaviour
{
    // Managers for AR subsystems
    public ARTrackedImageManager imageManager;
    public ARAnchorManager anchorManager;
    public HUDManager hudManager;

    // Prefabs for different projectiles
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


    public GameObject trackedTarget; // Store the spawned cube reference
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

    public AudioManagerVuforia audioManagerVuforia;

    // Dictionary to map anchors to snow particle effects
    private Dictionary<ARAnchor, GameObject> anchorToSnowParticleMap = new Dictionary<ARAnchor, GameObject>();
    private int snowParticleCount = 0;

    void Start()
    {
        bombButton.onClick.AddListener(FireBomb);
        bulletButton.onClick.AddListener(FireBullet);
        badmintonButton.onClick.AddListener(FireBadminton);
        golfButton.onClick.AddListener(FireGolf);
        boxingButton.onClick.AddListener(FireBoxing);
        fencingButton.onClick.AddListener(FireFencing);
        shieldButton.onClick.AddListener(() => ToggleShield(5));
    }

    void Update()
    {
    }

    void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnImageChanged);
        anchorManager.trackablesChanged.AddListener(OnTrackablesChanged);
    }

    void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnImageChanged);
        anchorManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs) // Spawn, update, and remove the target cube
    {
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnTarget(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTarget(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            RemoveTarget();
        }
    }

    public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARAnchor> eventArgs) // Update the snow particle effects
    {
        foreach (var anchor in eventArgs.updated)
        {
            if (anchorToSnowParticleMap.TryGetValue(anchor, out GameObject snowParticleInstance))
            {
                snowParticleInstance.transform.position = anchor.transform.position;
            }
        }
    }

    void SpawnTarget(ARTrackedImage trackedImage)
    {
        if (trackedTarget == null)
        {
            trackedTarget = Instantiate(targetPrefab, trackedImage.transform.position, Quaternion.identity);
        }
        else
        {
            trackedTarget.transform.position = trackedImage.transform.position;
        }
    }

    void UpdateTarget(ARTrackedImage trackedImage)
    {
        if (trackedTarget != null)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                // Enable the cube and update its position/rotation
                trackedTarget.SetActive(true);
                trackedTarget.transform.position = trackedImage.transform.position;
                trackedTarget.transform.rotation = trackedImage.transform.rotation;
            }
            else
            {
                // Disable the cube when tracking is lost
                trackedTarget.SetActive(false);
            }
        }
    }

    void RemoveTarget()
    {
        if (trackedTarget != null)
        {
            Destroy(trackedTarget); // Destroy the instantiated cube
            trackedTarget = null; // Remove the reference
        }
    }

    public void FireBomb()
    {
        GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, MainCamera.rotation);
        audioManagerVuforia.PlayBombSound(); // Play the bomb sound
        if (trackedTarget != null && IsTargetInView(trackedTarget.transform))
        {
            bomb.GetComponent<ArcProjectile>().FireArcProjectile(trackedTarget.transform, true, 5 * snowParticleCount);
            CreateSnowParticleEffect(trackedTarget.transform.position);
            hudManager.ShowDamage();
        }
        else
        {
            // Calculate a position 3 meters in front of the camera
            Vector3 forwardPosition = MainCamera.position + MainCamera.forward * 3f;
            GameObject tempTarget = new GameObject("TempTarget");
            tempTarget.transform.position = forwardPosition;
            bomb.GetComponent<ArcProjectile>().FireArcProjectile(tempTarget.transform, false, 0);
            //CreateSnowParticleEffect(tempTarget.transform.position);
            Destroy(tempTarget, 3f); // Clean up the temporary target after 3 seconds
            hudManager.ShowMiss();
        }
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(MainCamera.forward, Vector3.up));
        audioManagerVuforia.PlayGunSound(); // Play the gun sound
        if (trackedTarget != null && IsTargetInView(trackedTarget.transform))
        {
            bullet.GetComponent<StraightProjectiles>().FireStraightProjectile(trackedTarget.transform, true, 5 * snowParticleCount);
            hudManager.ShowDamage();
        }
        else
        {
            FireAtTemporaryTarget(bullet, bulletSpawnPoint, false);
            hudManager.ShowMiss();
        }
    }

    public void FireBadminton()
    {
        GameObject shuttle = Instantiate(badmintonPrefab, badmintonSpawnPoint.position, MainCamera.rotation);
        audioManagerVuforia.PlayBadmintonSound(); // Play the badminton sound
        if (trackedTarget != null && IsTargetInView(trackedTarget.transform))
        {
            shuttle.GetComponent<ArcProjectile>().FireArcProjectile(trackedTarget.transform, true, 5 * snowParticleCount);
            hudManager.ShowDamage();
        }
        else
        {
            FireAtTemporaryTarget(shuttle, badmintonSpawnPoint, true);
            hudManager.ShowMiss();
        }
    }

    public void FireBoxing()
    {
        GameObject glove = Instantiate(boxingPrefab, boxingSpawnPoint.position, Quaternion.LookRotation(MainCamera.forward, Vector3.up));
        audioManagerVuforia.PlayBoxingSound(); // Play the boxing sound
        if (trackedTarget != null && IsTargetInView(trackedTarget.transform))
        {
            glove.GetComponent<StraightProjectiles>().FireStraightProjectile(trackedTarget.transform, true, 5 * snowParticleCount);
            hudManager.ShowDamage();
        }
        else
        {
            FireAtTemporaryTarget(glove, boxingSpawnPoint, false);
            hudManager.ShowMiss();
        }
    }

    public void FireGolf()
    {
        GameObject missile = Instantiate(golfPrefab, golfSpawnPoint.position, MainCamera.rotation);
        audioManagerVuforia.PlayGolfSound(); // Play the golf sound
        if (trackedTarget != null && IsTargetInView(trackedTarget.transform))
        {
            missile.GetComponent<ArcProjectile>().FireArcProjectile(trackedTarget.transform, true, 5 * snowParticleCount);
            hudManager.ShowDamage();
        }
        else
        {
            FireAtTemporaryTarget(missile, golfSpawnPoint, true);
            hudManager.ShowMiss();
        }
    }

    public void FireFencing()
    {
        GameObject bullet = Instantiate(fencingPrefab, fencingSpawnPoint.position, Quaternion.LookRotation(MainCamera.forward, Vector3.up));
        audioManagerVuforia.PlayFencingSound(); // Play the fencing sound
        if (trackedTarget != null && IsTargetInView(trackedTarget.transform))
        {
            bullet.GetComponent<StraightProjectiles>().FireStraightProjectile(trackedTarget.transform, true, 5 * snowParticleCount);
            hudManager.ShowDamage();
        }
        else
        {
            FireAtTemporaryTarget(bullet, fencingSpawnPoint, false);
            hudManager.ShowMiss();
        }
    }

    public void ToggleShield(int shieldHp)
    {
        if (shieldHp > 0)
        {
            if (trackedTarget != null && IsTargetInView(trackedTarget.transform))
            {
                if (shieldInstance == null)
                {
                    // Spawn the shield
                    shieldInstance = Instantiate(shieldPrefab, trackedTarget.transform.position, trackedTarget.transform.rotation);
                    shieldInstance.transform.parent = trackedTarget.transform;
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
        audioManagerVuforia.PlayReloadSound(); // Play the reload sound
        GunRecoil gunController = MainCamera.GetComponentInChildren<GunRecoil>();
        gunController.Reload();
        if (snowParticleCount > 0)
        {
            SpawnsDamagePopups.Instance.DamageDone(5 * snowParticleCount, trackedTarget.transform.position, false);
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

    private async void CreateSnowParticleEffect(Vector3 position)
    {
        if (anchorManager != null)
        {
            var anchor = await anchorManager.TryAddAnchorAsync(new Pose(position, Quaternion.identity));
            if (anchor.status.IsSuccess())
            {
                GameObject snowParticleInstance = Instantiate(snowParticlePrefab, position, Quaternion.identity);
                snowParticleInstance.transform.SetParent(anchor.value.transform);
                SnowParticleProjectile snowParticleProjectile = snowParticleInstance.GetComponent<SnowParticleProjectile>();
                if (snowParticleProjectile != null)
                {
                    snowParticleProjectile.target = trackedTarget.transform;
                }

            }
            else
            {
                Debug.LogError("Failed to create anchor for snow particle effect.");
            }
        }
    }

    private void FireAtTemporaryTarget(GameObject missile, Transform spawnPoint, bool isArcProjectile)
    {
        // Calculate a position 3 meters in front of the camera
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
        Debug.Log($"Snow particles in range: {snowParticleCount}");
    }

    // Method to decrement the counter
    public void DecrementSnowParticleCount()
    {
        snowParticleCount = Mathf.Max(0, snowParticleCount - 1); // Ensure it doesn't go below 0
        Debug.Log($"Snow particles in range: {snowParticleCount}");
    }

    // Method to get the current count
    public int GetSnowParticleCount()
    {
        return snowParticleCount;
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