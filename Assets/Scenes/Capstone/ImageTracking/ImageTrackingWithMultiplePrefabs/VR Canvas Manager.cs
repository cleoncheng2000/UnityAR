using UnityEngine;
using UnityEngine.UI;

public class VRCanvasManager : MonoBehaviour
{
    public RawImage leftImage;  // Reference to the left RawImage
    public RawImage rightImage; // Reference to the right RawImage

    [Header("Image Settings")]
    public float distanceBetweenImages = 1.0f; // Distance between the two images
    public Vector2 imageSize = new Vector2(200, 200); // Size of both images (width, height)

    void Start()
    {
        UpdateImagePositions();
        UpdateImageSizes();
    }

    void Update()
    {
        // Optionally, you can call these methods in Update if you want real-time adjustments
        UpdateImagePositions();
        UpdateImageSizes();
    }

    // Method to update the positions of the images
    public void UpdateImagePositions()
    {
        if (leftImage != null && rightImage != null)
        {
            // Position the left image
            leftImage.rectTransform.localPosition = new Vector3(-distanceBetweenImages / 2, 0, 0);

            // Position the right image
            rightImage.rectTransform.localPosition = new Vector3(distanceBetweenImages / 2, 0, 0);
        }
    }

    // Method to update the sizes of the images
    public void UpdateImageSizes()
    {
        if (leftImage != null)
        {
            leftImage.rectTransform.sizeDelta = imageSize;
        }

        if (rightImage != null)
        {
            rightImage.rectTransform.sizeDelta = imageSize;
        }
    }
}
