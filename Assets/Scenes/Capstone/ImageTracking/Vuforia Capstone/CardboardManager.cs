using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Management;
using Google.XR.Cardboard;

public class CardboardManager : MonoBehaviour
{
    public Button launchButton;
    public Canvas UICanvas;
    private Google.XR.Cardboard.XRLoader cardboardLoader;

    void Start()
    {
        // Ensure the button is active at the start
        launchButton.gameObject.SetActive(true);
        launchButton.onClick.AddListener(LaunchGoogleCardboard);

    }


    public void LaunchGoogleCardboard()
    {
        UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
        UICanvas.worldCamera = Camera.main;
        UICanvas.planeDistance = 1.0f;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Initialize and start the XR loader using Unity's XR Management system
            XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                launchButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to initialize Cardboard XR loader.");
            }
        }
    }

    void Update()
    {
        // Check if the user has touched the screen
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // Stop and deinitialize the XR loader
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();

                // Reactivate the launch button
                launchButton.gameObject.SetActive(true);
            }
        }
    }
}