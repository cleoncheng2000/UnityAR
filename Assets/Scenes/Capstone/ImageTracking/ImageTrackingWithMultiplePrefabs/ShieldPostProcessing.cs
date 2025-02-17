using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ShieldPostProcessing : MonoBehaviour
{
    private Volume shieldVolume;  // Assign the post-processing Volume
    public Button selfShieldButton; // Assign the button to toggle the shield
    private Vignette vignette;

    private bool isActive = false;

    void Start()
    {
        shieldVolume = GetComponent<Volume>();
        if (shieldVolume != null && shieldVolume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = 0f; // Start off
            vignette.color.value = Color.blue; // Set the color to blue
        }
        else
        {
            Debug.LogError("Vignette effect not found in the volume profile.");
        }

        if (selfShieldButton != null)
        {
            selfShieldButton.onClick.AddListener(ToggleShield);
        }
        else
        {
            Debug.LogError("SelfShieldButton is not assigned.");
        }
    }

    public void ToggleShield()
    {
        if (vignette != null)
        {
            isActive = !isActive;
            vignette.intensity.value = isActive ? 0.5f : 0f; // Adjust glow
        }
    }
}