using TMPro;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.8f;
    private CharacterController characterController;

    [Header("Fishing Mechanic")]
    public bool isFishing = false;
    public bool fishOnHook = false;
    public GameObject baitPrefab; // Reference to the bait prefab
    private GameObject currentBait; // Stores the instantiated bait
    private WaterDetector WaterDetector;
    private bool isReeling = false;

    // Adjust the fishing reel timings
    public float reelingTimer = 0f;
    public float maxReelingTimer = 3f;
    public float maxAllowableReelingTimer = 10f;

    public float caughtFishDisplayTime = 3f; // Time to display caught fish message
    public float caughtFishTimer = 0f;
    private bool waitingForBite = false;

    [Header("UI")]
    public TextMeshProUGUI reelingCountdownText;
    public TextMeshProUGUI allowableCountdownText;
    public TextMeshProUGUI caughtFishText;
    private Vector3 gravityVelocity = Vector3.zero;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        WaterDetector = GetComponent<WaterDetector>();
        if (caughtFishText != null)
        {
            caughtFishText.gameObject.SetActive(false);
        }
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
            reelingCountdownText.gameObject.SetActive(false); 
            allowableCountdownText.gameObject.SetActive(false); 
        }

        // Hide the caught fish text after a set duration
        if (caughtFishText.gameObject.activeSelf)
        {
            caughtFishTimer += Time.deltaTime;
            if (caughtFishTimer >= caughtFishDisplayTime)
            {
                caughtFishText.gameObject.SetActive(false);
                caughtFishTimer = 0f; // Reset the timer
            }
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

            if (currentBait != null)
                Destroy(currentBait);

            Vector3 castPosition = transform.position + transform.forward * 5f;
            castPosition.y = 0.1f; 

            currentBait = Instantiate(baitPrefab, castPosition, Quaternion.identity);
            currentBait.tag = "Bait";

            StartCoroutine(CheckBiteAfterDelay(2f));
        }
        else if (WaterDetector == null || !WaterDetector.CanFish())
        {
            Debug.Log("You must be near water to fish.");
        }
    }

    private IEnumerator CheckBiteAfterDelay(float delay)
    {
        waitingForBite = true;
        yield return new WaitForSeconds(delay);

        float catchChance = Random.Range(0f, 1f);

        if (catchChance > 0.3f)
        {
            Debug.Log("Fish On! Press 'R' to reel in.");
            fishOnHook = true;
            reelingTimer = 0f;
            isReeling = false;  
        }
        else
        {
            Debug.Log("Nothing took the bait. Reel it back in and try again.");
            fishOnHook = false;
        }

        waitingForBite = false;
    }


    public void ReelFish()
    {
        if (isFishing && fishOnHook)
        {
            isReeling = true; 
            reelingTimer += Time.deltaTime;

            if (reelingTimer > maxAllowableReelingTimer) 
            {
                Debug.Log("You reeled for too long! The fish got away.");
                LoseFish();
            }
        }
    }

    public void StopReeling() 
    {
        if (isFishing && !waitingForBite)
        {
            if (fishOnHook)
            {
                if (reelingTimer >= maxReelingTimer && reelingTimer <= maxAllowableReelingTimer)
                {
                    CatchFish();
                }
                else if (reelingTimer < maxReelingTimer) 
                {
                    Debug.Log("You released R too early. The fish got away.");
                    LoseFish();
                }
            }
            else
            {
                Debug.Log("You reeled in the line. Nothing was caught.");
                ResetFishingState();
            }
        }
        isReeling = false; 
    }

    private void CatchFish()
    {
        Debug.Log("You caught a fish!");
        DetermineCatch();
        ResetFishingState();
    }

    private void LoseFish()
    {
        ResetFishingState();
    }

    private void ResetFishingState()
    {
        isFishing = false;
        fishOnHook = false;
        reelingTimer = 0f;
        isReeling = false;

        if (currentBait != null)
            Destroy(currentBait);

        if (reelingCountdownText != null && allowableCountdownText != null)
        {
            reelingCountdownText.gameObject.SetActive(false);
            allowableCountdownText.gameObject.SetActive(false);
        }
    }

    private void UpdateCountdownUI()
    {
        if (reelingCountdownText != null && allowableCountdownText != null)
        {
            reelingCountdownText.gameObject.SetActive(true);
            allowableCountdownText.gameObject.SetActive(true);

            float releaseTimeLeft = Mathf.Clamp(maxReelingTimer - reelingTimer, 0, maxReelingTimer);
            reelingCountdownText.text = $"Reel Until Ready: {releaseTimeLeft:F1} seconds";

            float allowableTimeLeft = Mathf.Clamp(maxAllowableReelingTimer - reelingTimer, 0, maxAllowableReelingTimer);
            allowableCountdownText.text = $"Max Reel Time Left: {allowableTimeLeft:F1} seconds";
        }
    }

    private void DetermineCatch()
    {
        int catchResult = Random.Range(0, 100);

        string caughtItem;

        if (catchResult < 34)
        {
            caughtItem = "You caught a Bass!";
        }
        else if (catchResult < 67)
        {
            caughtItem = "You caught a Trout!";
        }
        else
        {
            caughtItem = "You caught a Boot!";
        }

        Debug.Log(caughtItem);

        if (caughtFishText != null)
        {
            caughtFishText.text = caughtItem;
            caughtFishText.gameObject.SetActive(true);

            caughtFishTimer = 0f; // Reset the timer
        }
    }
}
