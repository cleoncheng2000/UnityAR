using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Management;
using Google.XR.Cardboard;

public class CardboardManager : MonoBehaviour
{
    public Button launchButton;
    private Google.XR.Cardboard.XRLoader cardboardLoader;

    void Start()
    {
        // Ensure the button is active at the start
        launchButton.gameObject.SetActive(true);
        launchButton.onClick.AddListener(LaunchGoogleCardboard);
    }


    public void LaunchGoogleCardboard()
    {
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
        if (Google.XR.Cardboard.Api.IsCloseButtonPressed)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                cardboardLoader.Stop();
                cardboardLoader.Deinitialize();
                launchButton.gameObject.SetActive(true);
            }
        }
    }
}