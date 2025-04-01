using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.8f;
    private CharacterController characterController;

    [Header("Fishing Mechanic")]
    private bool isFishing = false;
    private bool fishOnHook = false;
    // Adjust the fishing reel timings
    public float reelingTimer = 0f;
    public float maxReelingTimer = 3f;
    public float maxAllowableReelingTimer = 10f; // Maximum time allowed to reel in a fish
    private WaterDetector WaterDetector;
    private bool isReeling = false;

    [Header("UI")]
    public TextMeshProUGUI reelingCountdownText;
    public TextMeshProUGUI allowableCountdownText;
    private Vector3 gravityVelocity = Vector3.zero;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        // grab the water collision check to fish on the water
        WaterDetector = GetComponent<WaterDetector>();
    }

    void Update()
    {
        ApplyGravity();

        if (fishOnHook && isReeling)
        {
            UpdateCountdownUI();
        }
        else
        {
            reelingCountdownText.gameObject.SetActive(false); // Hide countdown UI when not actively reeling
            allowableCountdownText.gameObject.SetActive(false); // Hide countdown UI when not actively reeling        
        }
    }

    public void MoveWithCC(Vector3 direction)
    {
        characterController.Move(direction * speed * Time.deltaTime);
        if (direction != Vector3.zero)
        {
            transform.LookAt(transform.position + direction);
        }
    }

    void ApplyGravity()
    {

        gravityVelocity.y += gravity * Time.deltaTime;
        characterController.Move(gravityVelocity * Time.deltaTime);
    }

    public void CastFishingRod()
    {
        if (!isFishing && WaterDetector != null && WaterDetector.CanFish()) 
        {
            Debug.Log("Casting Fishing Rod...");
            isFishing = true;

            float catchChance = Random.Range(0f, 1f);

            if (catchChance > 0.3f) // 70% chance of hooking a fish
            {
                Debug.Log("Fish On! Press 'R' to reel in.");
                fishOnHook = true;
                reelingTimer = 0f;
                isReeling = false;  // Reset reeling state
            }
            else
            {
                Debug.Log("Nothing took the bait. Try again.");
                isFishing = false;
            }
        }
        else if (!WaterDetector.CanFish())
        {
            Debug.Log("You must be near water to fish.");
        }
      
    }

    public void ReelFish()
    {
        if (isFishing && fishOnHook)
        {
            Debug.Log("Reeling...");
            isReeling = true; 
            reelingTimer += Time.deltaTime;

            // Only checking if the player exceeds the maximum allowable time WHILE reeling
            if (reelingTimer > maxAllowableReelingTimer) 
            {
                Debug.Log("You reeled for too long!");
                LoseFish();
            }
        }
    }


    public void StopReeling() 
    {
        if (isFishing && fishOnHook)
        {
            if (reelingTimer >= maxReelingTimer && reelingTimer <= maxAllowableReelingTimer)
            {
                CatchFish();
            }
            else if (reelingTimer < maxReelingTimer) 
            {
                Debug.Log("You lost the fish! You released R too early.");
                LoseFish();
            }
        }
        isReeling = false; 
    }


    private void CatchFish()
    {
        Debug.Log("You caught a fish!");
        ResetFishingState();
    }

    private void LoseFish()
    {
        Debug.Log("The fish got away...");
        ResetFishingState();
    }

    private void ResetFishingState()
    {
        isFishing = false;
        fishOnHook = false;
        reelingTimer = 0f;
        isReeling = false;
    }

    private void UpdateCountdownUI()
    {
        if (reelingCountdownText != null && allowableCountdownText != null)
        {
            reelingCountdownText.gameObject.SetActive(true);
            allowableCountdownText.gameObject.SetActive(true);

            // Display how much time is left to catch the fish (0 to 3 seconds)
            float releaseTimeLeft = Mathf.Clamp(maxReelingTimer - reelingTimer, 0, maxReelingTimer);
            reelingCountdownText.text = $"Reel Until Ready: {releaseTimeLeft:F1} seconds";

            // Display how much time is left before reeling too long (0 to 10 seconds)
            float allowableTimeLeft = Mathf.Clamp(maxAllowableReelingTimer - reelingTimer, 0, maxAllowableReelingTimer);
            allowableCountdownText.text = $"Max Reel Time Left: {allowableTimeLeft:F1} seconds";
        }
    }

}
