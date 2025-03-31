using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;     // The player to follow
    public Vector3 offset = new Vector3(0, 10, -10); 
    public float smoothSpeed = 0.125f;  // Smooth the camera movement

    // LateUpdate is called after all Update functions
    // This is where we want to move the camera
    // to ensure it follows the player smoothly
    void LateUpdate()
    {
        if (player == null) return; 

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
    
    }
}
