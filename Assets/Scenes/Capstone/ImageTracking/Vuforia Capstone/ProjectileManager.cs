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
using M2MqttUnity;
using M2MqttUnity.Examples;
using Newtonsoft.Json;

using UnityEngine.Android;
using System;

public class ProjectileManager : MonoBehaviour
{
    public HUDManager hudManager;
    public PlayerManager playerManager; // Reference to the player manager
    public GameObject targetRedPrefab;
    public GameObject targetWhitePrefab;
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
    
    public ImageTargetBehaviour targetBehaviour; // Reference to the Vuforia observer behaviour
    public ImageTargetBehaviour worldCentreTarget; // Reference to the tracked target
    public M2MqttUnityTest MqttUnity;
    public List<GameObject> SnowParticlePositions = new List<GameObject>(); // Store the positions of snow particles
    // Dictionary to map anchors to snow particle effects
    private int snowParticleCount = 0; // Counter for snow particles in range
    private bool currentStatus = false; // Track the current status of the target

    private bool curVisibilityStatus = false;
    private bool previousVisibilityStatus = false; // Track the visibility status of the target
    private bool truePreviousVisbilityStatus = false; // Track the previous visibility status of the target
    private Coroutine visibilityMonitorCoroutine;

    private Coroutine snowBombsMonitorCoroutine; // Coroutine to monitor snow bombs

    public event Action<bool, int> OnVisibilityAndSnowBombsStatusChanged; // Event to notify visibility and snow bombs status changes

    void Start()
    {
        bombButton.onClick.AddListener(FireBomb);
        bulletButton.onClick.AddListener(FireBullet);
        badmintonButton.onClick.AddListener(FireBadminton);
        golfButton.onClick.AddListener(FireGolf);
        boxingButton.onClick.AddListener(FireBoxing);
        fencingButton.onClick.AddListener(FireFencing);
        shieldButton.onClick.AddListener(() => ToggleShield(5));
        targetBehaviour.OnTargetStatusChanged += onTargetStatusChanged; // Subscribe to the target status change event
        visibilityMonitorCoroutine = StartCoroutine(MonitorVisibilityStatus()); // Start the coroutine to monitor visibility status
        snowBombsMonitorCoroutine = StartCoroutine(MonitorSnowBombsStatus()); // Start the coroutine to monitor snow bombs
    }

    private IEnumerator MonitorVisibilityStatus()
    {
        while (true)
        {
            curVisibilityStatus = IsTargetInView(targetBehaviour.transform);
            Debug.Log($"Initial visibility check: {curVisibilityStatus}");

            // If previously visible but now not, wait before confirming
            if (truePreviousVisbilityStatus && !curVisibilityStatus)
            {
                yield return new WaitForSeconds(1.0f);
                curVisibilityStatus = IsTargetInView(targetBehaviour.transform);
                Debug.Log($"Rechecked visibility after 1 second: {curVisibilityStatus}");
            }

            if (truePreviousVisbilityStatus != curVisibilityStatus)
            {
                truePreviousVisbilityStatus = curVisibilityStatus;
                previousVisibilityStatus = curVisibilityStatus;
                NotifyStatusChange();
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void NotifyStatusChange()
    {
        OnVisibilityAndSnowBombsStatusChanged?.Invoke(truePreviousVisbilityStatus, snowParticleCount);
    }

    private IEnumerator MonitorSnowBombsStatus()
    {
        while (true)
        {
            int newSnowBombsStatus = GetSnowParticleCount();
            if (newSnowBombsStatus != snowParticleCount)
            {
                snowParticleCount = newSnowBombsStatus;
                snowParticleCountText.text = snowParticleCount.ToString(); // Update the UI text with the new count
                NotifyStatusChange();
            }
            if (snowParticleCount > 0)
            {
                targetWhitePrefab.SetActive(true);
                targetRedPrefab.SetActive(false);
            }
            else
            {
                targetWhitePrefab.SetActive(false);
                targetRedPrefab.SetActive(true);
            }
            yield return new WaitForSeconds(0.5f); // Check every 0.5 seconds
        }
    }

    public bool getVisbilityStatus()
    {
        return truePreviousVisbilityStatus;
    }

    public int getSnowBombs()
    {
        return snowParticleCount;
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
        if (currentStatus == true && IsTargetInView(targetBehaviour.transform) && playerManager.GetCurrentPlayer().bombs > 0)
        {
            bomb.GetComponent<ArcProjectile>().FireArcProjectile(targetBehaviour.transform, true, 5 * snowParticleCount);
            CreateSnowParticleEffect(targetBehaviour.transform.position);
            hudManager.ShowDamage();
        }
        else
        {
            // Calculate a position 3 meters in front of the camera
            Vector3 forwardPosition = MainCamera.position + MainCamera.forward * 3f;
            GameObject tempTarget = new GameObject("TempTarget");
            tempTarget.transform.position = forwardPosition;
            bomb.GetComponent<ArcProjectile>().FireArcProjectile(tempTarget.transform, false, 0);
            Destroy(tempTarget, 3f); // Clean up the temporary target after 3 seconds
            hudManager.ShowMiss();
        }
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(MainCamera.forward, Vector3.up));
        GunRecoil gunController = MainCamera.GetComponentInChildren<GunRecoil>();
        gunController.Shoot();
        audioManager.PlayGunSound(); // Play the gun sound
        if (targetBehaviour != null && IsTargetInView(targetBehaviour.transform) && playerManager.GetCurrentPlayer().bullets > 0)
        {
            bullet.GetComponent<StraightProjectiles>().FireStraightProjectile(targetBehaviour.transform, true, 5 * snowParticleCount);
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
        audioManager.PlayBadmintonSound(); // Play the badminton sound
        if (targetBehaviour != null && IsTargetInView(targetBehaviour.transform))
        {
            shuttle.GetComponent<ArcProjectile>().FireArcProjectile(targetBehaviour.transform, true, 5 * snowParticleCount);
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
        audioManager.PlayBoxingSound(); // Play the boxing sound
        if (targetBehaviour != null && IsTargetInView(targetBehaviour.transform))
        {
            glove.GetComponent<StraightProjectiles>().FireStraightProjectile(targetBehaviour.transform, true, 5 * snowParticleCount);
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
        audioManager.PlayGolfSound(); // Play the golf sound
        if (targetBehaviour != null && IsTargetInView(targetBehaviour.transform))
        {
            missile.GetComponent<ArcProjectile>().FireArcProjectile(targetBehaviour.transform, true, 5 * snowParticleCount);
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
        audioManager.PlayFencingSound(); // Play the fencing sound
        if (targetBehaviour != null && IsTargetInView(targetBehaviour.transform))
        {
            bullet.GetComponent<StraightProjectiles>().FireStraightProjectile(targetBehaviour.transform, true, 5 * snowParticleCount);
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
            if (targetBehaviour != null && IsTargetInView(targetBehaviour.transform))
            {
                if (shieldInstance == null)
                {
                    // Spawn the shield
                    shieldInstance = Instantiate(shieldPrefab, targetBehaviour.transform.position, targetBehaviour.transform.rotation);
                    shieldInstance.transform.parent = targetBehaviour.transform;
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
            hudManager.ShowDamage();
        }
    }

    public bool IsTargetInView(Transform target)
    {
        if (target == null)
        {
            return false;
        }
        Vector3 viewportPoint = MainCamera.GetComponent<Camera>().WorldToViewportPoint(target.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0 && targetBehaviour.TargetStatus.Status == Status.TRACKED;
    }

    private void CreateSnowParticleEffect(Vector3 position)
    {
        // Instantiate the snow particle at the given position
        GameObject snowParticleInstance = Instantiate(snowParticlePrefab, position, Quaternion.identity);

        // Set the snow particle instance as a child of the worldCentreTarget
        snowParticleInstance.transform.SetParent(worldCentreTarget.transform);


        // Add the snow particle position to the list
        SnowParticlePositions.Add(snowParticleInstance);

        Debug.Log("Snow particle effect created and set as a child of worldCentreTarget.");
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
        Vector3 observerPosition = targetBehaviour.transform.position;

        // Initialize a counter for snow particles within range
        int count = 0;

        // Iterate through the list of snow particle positions
        foreach (GameObject snowParticle in SnowParticlePositions)
        {
            // Calculate the distance between the observer and the snow particle
            float distance = Vector3.Distance(observerPosition, snowParticle.transform.position);

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