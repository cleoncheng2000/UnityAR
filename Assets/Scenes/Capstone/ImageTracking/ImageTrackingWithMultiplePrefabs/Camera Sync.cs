using UnityEngine;

public class CameraSync : MonoBehaviour
{
    public Transform rightEye; // Drag the RightEyeCamera here in the Inspector

    void Update()
    {
        if (rightEye != null)
        {
            transform.position = rightEye.position;
            transform.rotation = rightEye.rotation;
        }
    }
}
