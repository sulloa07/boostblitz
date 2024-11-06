using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public Transform rocket;

    // Determines how smoothly the camera follows the rocket
    public float smoothSpeed = 0.125f;

    // The positional offset from the rocket to maintain in the camera view
    public Vector3 offset;

    void LateUpdate()
    {
        if (rocket != null)
        {
            // Calculate the desired position with the offset applied
            Vector3 desiredPosition = new Vector3(0, rocket.position.y, -10) + offset;

            // Smoothly interpolate between the current camera position and the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Update the camera's position to the smoothed position
            transform.position = smoothedPosition;
        }
    }
}
