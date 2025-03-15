using UnityEngine;
using UnityEngine.UI;

public class StereoscopicSplitScreen : MonoBehaviour
{
    public Camera mainCamera;      // Assign your Main Camera (AR Camera)
    public RenderTexture renderTexture; // Assign your Render Texture
    public Canvas canvas;          // Assign your Canvas

    private RawImage leftEye;
    private RawImage rightEye;

    void Start()
    {
        if (mainCamera == null || renderTexture == null || canvas == null)
        {
            Debug.LogError("StereoscopicSplitScreen: Please assign Main Camera, Render Texture, and Canvas.");
            return;
        }

        // Assign Render Texture to Camera
        mainCamera.targetTexture = renderTexture;

        // Create Left Eye View
        leftEye = CreateEyeView("LeftEye", new Rect(0, 0, 0.5f, 1), new Vector2(-Screen.width / 4, 0));

        // Create Right Eye View
        rightEye = CreateEyeView("RightEye", new Rect(0.5f, 0, 0.5f, 1), new Vector2(Screen.width / 4, 0));
    }

    RawImage CreateEyeView(string name, Rect uvRect, Vector2 position)
    {
        GameObject eyeObject = new GameObject(name);
        eyeObject.transform.SetParent(canvas.transform, false);

        RawImage rawImage = eyeObject.AddComponent<RawImage>();
        rawImage.texture = renderTexture;   // Use the same Render Texture for both eyes
        rawImage.uvRect = uvRect;           // Split the UV Rect

        RectTransform rectTransform = rawImage.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(Screen.width / 2, Screen.height);  // Half screen width
        rectTransform.anchoredPosition = position;

        return rawImage;
    }
}
