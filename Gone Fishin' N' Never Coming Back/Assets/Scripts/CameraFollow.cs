using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  
    public Vector3 offset = new Vector3(0, 5, -5); 
    public float smoothSpeed = 0.125f;
    public float rotateSpeed = 3f;
    public float pitchSpeed = 2f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private Vector3 currentOffset;
    private bool isRotating = false;
    private float pitch = 20f;

    void Start()
    {
        if (player != null)
            transform.position = player.position + offset;

        currentOffset = offset;
    }

    void LateUpdate()
    {
        if (player == null) return;

        if (Input.GetMouseButton(1))
        {
            isRotating = true;

            float mouseX = Input.GetAxis("Mouse X");
            Quaternion yawRot = Quaternion.AngleAxis(mouseX * rotateSpeed, Vector3.up);
            currentOffset = yawRot * currentOffset;

            float mouseY = Input.GetAxis("Mouse Y");
            pitch -= mouseY * pitchSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        else if (isRotating)
        {
            currentOffset = Vector3.Lerp(currentOffset, offset, Time.deltaTime * 5f);
            pitch = Mathf.Lerp(pitch, 20f, Time.deltaTime * 5f);

            if (Vector3.Distance(currentOffset, offset) < 0.05f && Mathf.Abs(pitch - 20f) < 0.5f)
            {
                currentOffset = offset;
                pitch = 20f;
                isRotating = false;
            }
        }

        Vector3 desiredPosition = player.position + currentOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        Quaternion lookRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(pitch, lookRotation.eulerAngles.y, 0f), Time.deltaTime * 5f);
    }
}
