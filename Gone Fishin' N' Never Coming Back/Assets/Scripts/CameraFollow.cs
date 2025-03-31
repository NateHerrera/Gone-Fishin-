using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;       // The player to follow
    public Vector3 offset = new Vector3(0, 10, -10); // Set your desired camera position offset
    public float smoothSpeed = 0.125f;  // Smoothness of the camera movement

    void LateUpdate()
    {
        if (player == null) return; // Prevents the error you saw before

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Uncomment the next line if you want the camera to always face the player.
        // transform.LookAt(player);
    }
}
