using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnchorCreator : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private ARAnchorManager anchorManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField] private GameObject anchorPrefab;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.Planes))
                {
                    CreateAnchorAtHit();
                }
            }
        }
    }

    private async void CreateAnchorAtHit()
    {
        ARRaycastHit hit = hits[0]; // Take the first valid hit
        Pose hitPose = hit.pose;

        Debug.Log("Attempting to create an anchor...");

        var anchorResult = await anchorManager.TryAddAnchorAsync(hitPose);

        if (anchorResult.status.IsSuccess())
        {
            ARAnchor anchor = anchorResult.value;

            // Instantiate the prefab and parent it to the anchor
            GameObject spawnedObject = Instantiate(anchorPrefab, hitPose.position, hitPose.rotation);
            spawnedObject.transform.parent = anchor.transform;

            Debug.Log("Anchor successfully created at: " + hitPose.position);
        }
        else
        {
            Debug.LogWarning("Failed to create anchor. Error: " + anchorResult.status);
        }
    }
}
