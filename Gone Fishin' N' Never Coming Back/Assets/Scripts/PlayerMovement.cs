using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpSpeed = 10f;
    private CharacterController characterController;

    [Header("Fishing Mechanic")]
    public GameObject fishingRod; // Reference to the fishing rod or cast point

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        ApplyGravity();
    }

    public void MoveWithCC(Vector3 direction)
    {
        characterController.Move(direction * speed * Time.deltaTime);
        if (direction != Vector3.zero)
        {
            transform.LookAt(transform.position + direction);
        }
    }

    private Vector3 gravityVelocity = Vector3.zero;

    void ApplyGravity()
    {
        if (characterController.isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
            return;
        }

        gravityVelocity.y += gravity * Time.deltaTime;
        characterController.Move(gravityVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            gravityVelocity.y = jumpSpeed;
        }
    }

    public void CastFishingRod()
    {
        Debug.Log("Casting Fishing Rod!");
        // Here you can trigger fishing animation or spawning fish
    }
}
