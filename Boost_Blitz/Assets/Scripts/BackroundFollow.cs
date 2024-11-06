using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    // Reference to the camera's Transform component
    public Transform cameraTransform;

    // The positional offset to apply to the background relative to the camera
    public Vector3 offset;

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Set the background's position to follow the camera with the specified offset
            transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, transform.position.z) + offset;
        }
    }
}
