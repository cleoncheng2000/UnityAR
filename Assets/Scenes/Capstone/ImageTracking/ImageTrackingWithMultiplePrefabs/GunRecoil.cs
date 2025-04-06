using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GunRecoil : MonoBehaviour
{
    public AnimationCurve recoilCurve;
    public AnimationCurve reloadCurve;

    public float recoilDuration = 0.5f;
    public float reloadDuration = 2f;
    // public Button shootButton;
    // public Button reloadButton;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isReloading = false;

    void Start()
    {

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        // shootButton.onClick.AddListener(Shoot);
        // reloadButton.onClick.AddListener(Reload);
    }

    public void Shoot()
    {
        if (!isReloading)
        {
            StartCoroutine(RecoilEffect());
        }
    }

    public void Reload()
    {
        if (!isReloading)
        {
            StartCoroutine(ReloadEffect());
        }
    }

    private IEnumerator RecoilEffect()
    {
        float elapsed = 0f;

        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;
            float recoilAmount = recoilCurve.Evaluate(elapsed / recoilDuration);
            transform.localPosition = originalPosition + new Vector3(0, recoilAmount * 0.2f, 0);
            yield return null;
        }

        transform.localPosition = originalPosition; // Reset after recoil
    }

    private IEnumerator ReloadEffect()
    {
        isReloading = true;
        float elapsed = 0f;

        // First phase: Apply the reload effect
        while (elapsed < reloadDuration)
        {
            elapsed += Time.deltaTime;
            float tiltAmount = reloadCurve.Evaluate(elapsed / reloadDuration);
            transform.localRotation = originalRotation * Quaternion.Euler(tiltAmount * -15f, 0, 0);
            yield return null;
        }

        // Second phase: Smoothly transition back to the original position and rotation
        elapsed = 0f;
        Vector3 currentPosition = transform.localPosition;
        Quaternion currentRotation = transform.localRotation;
        float transitionDuration = 0.5f; // Duration for the smooth transition

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            transform.localPosition = Vector3.Lerp(currentPosition, originalPosition, t);
            transform.localRotation = Quaternion.Lerp(currentRotation, originalRotation, t);
            yield return null;
        }

        // Ensure the final position and rotation are exactly the original values
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;

        isReloading = false;
    }
}
