using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Player Player;
    public Transform cameraTransform;
    public RodAnimator rodAnimator;
    public GameObject checklistPanel; // Reference to the checklist panel
    public PlayerMovement playerMovement;

    void Update()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0;

        Vector3 finalMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            finalMovement += cameraForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            finalMovement -= cameraForward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            finalMovement -= cameraRight;
        }

        if (Input.GetKey(KeyCode.D))
        {
            finalMovement += cameraRight;
        }

        finalMovement.Normalize();

         // Apply movement and switch animation state
        Player.MoveWithCC(finalMovement);

        if (finalMovement != Vector3.zero)
        {
            Player.ChangePlayerAnimationState(Player.walkAnimationState);
        }
        else
        {
            Player.ChangePlayerAnimationState(Player.idleAnimationState);
        }

        // Casting the fishing rod (Press F)
        if (Input.GetKeyDown(KeyCode.F))
        {
            Player.CastFishingRod();
            rodAnimator.TriggerCast();
        }

        /// Reeling in fish (Hold R)
        if (Input.GetKey(KeyCode.R))
        {
            Player.ReelFish();
        }

        // Stop Reeling (Release R)
        if (Input.GetKeyUp(KeyCode.R))
        {
            Player.StopReeling();
            rodAnimator.TriggerReel();
        }


    }

}
