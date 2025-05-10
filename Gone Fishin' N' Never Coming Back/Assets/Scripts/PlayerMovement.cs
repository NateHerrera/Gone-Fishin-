using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float gravity = -9.8f;
    public CharacterController characterController;

    [Header("Fishing Mechanic")] 
    public bool isFishing = false;
    public bool fishOnHook = false;
    public GameObject baitPrefab;
    private GameObject currentBait;
    private WaterDetector WaterDetector;
    private bool isReeling = false;

    public float reelingTimer = 0f;
    public float maxReelingTimer = 3f;
    public float maxAllowableReelingTimer = 10f;
    public float caughtFishDisplayTime = 3f;
    public float caughtFishTimer = 0f;
    private bool waitingForBite = false;

    [Header("Fishing Visuals")]
    public Transform rodTip;
    public LineRenderer fishingLine;
    public float castDistance = 20f;
    public float arcHeight = 300f;
    public GameObject targetCirclePrefab;
    private GameObject currentTargetCircle;
    public TextMeshProUGUI fishingStatusText;
    public float statusDisplayTime = 2f;
    private float statusTimer = 0f;

    [Header("UI")]
    public TextMeshProUGUI reelingCountdownText;
    public TextMeshProUGUI allowableCountdownText;
    public TextMeshProUGUI caughtFishText;
    private Vector3 gravityVelocity = Vector3.zero;

    [Header("Animations")]
    public AnimationStateChanger animationStateChanger;
    public string idleAnimationState = "Idle";
    public string walkAnimationState = "Walk";

    private int totalFishCaught = 0;
    private HashSet<string> uniqueFishCaught = new HashSet<string>();

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        WaterDetector = GetComponent<WaterDetector>();
        fishingLine = GetComponent<LineRenderer>();

        if (fishingLine != null)
        {
            fishingLine.positionCount = 2;
            fishingLine.startWidth = 0.02f;
            fishingLine.endWidth = 0.02f;
            fishingLine.material = new Material(Shader.Find("Sprites/Default"));
            fishingLine.startColor = Color.white;
            fishingLine.endColor = Color.white;
            fishingLine.enabled = true;
        }

        if (caughtFishText != null)
        {
            caughtFishText.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        ChangePlayerAnimationState(idleAnimationState);
    }

    void Update()
    {
        ApplyGravity();

        if (Input.GetKeyDown(KeyCode.F))
        {
            CastFishingRod();
        }

        if (fishOnHook && isReeling)
        {
            UpdateCountdownUI();
        }
        else
        {
            reelingCountdownText.gameObject.SetActive(false);
            allowableCountdownText.gameObject.SetActive(false);
        }

        if (fishingLine != null && currentBait != null)
        {
            fishingLine.enabled = true;
            fishingLine.SetPosition(0, rodTip.position);
            fishingLine.SetPosition(1, currentBait.transform.position);
        }

        if (caughtFishText.gameObject.activeSelf)
        {
            caughtFishTimer += Time.deltaTime;
            if (caughtFishTimer >= caughtFishDisplayTime)
            {
                caughtFishText.gameObject.SetActive(false);
                caughtFishTimer = 0f;
            }
        }

        if (fishingStatusText != null && fishingStatusText.gameObject.activeSelf)
        {
            statusTimer += Time.deltaTime;
            if (statusTimer >= statusDisplayTime)
            {
                fishingStatusText.gameObject.SetActive(false);
                statusTimer = 0f;
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
            isFishing = true;

            Vector3 start = rodTip.position;
            Vector3 direction = transform.forward;
            Vector3 target = start + direction * castDistance;
            target.y = 0.1f;

            if (currentTargetCircle == null && targetCirclePrefab != null)
            {
                currentTargetCircle = Instantiate(targetCirclePrefab, target, Quaternion.identity);
            }
            else if (currentTargetCircle != null)
            {
                currentTargetCircle.transform.position = target;
            }

            if (currentBait != null) Destroy(currentBait);

            currentBait = Instantiate(baitPrefab, start, Quaternion.identity);
            currentBait.tag = "Bait";

            StartCoroutine(MoveBaitWithArc(currentBait.transform, start, target, 1.2f, arcHeight));
            StartCoroutine(CheckBiteAfterDelay(2f));
        }
    }

    private IEnumerator MoveBaitWithArc(Transform bait, Vector3 start, Vector3 end, float duration, float height)
    {
        float time = 0f;
        while (time < 1f)
        {
            // Linear interpolation
            Vector3 linearPos = Vector3.Lerp(start, end, time);

            // Add arc (parabola)
            float arc = Mathf.Sin(time * Mathf.PI) * height;
            linearPos.y += arc;

            bait.position = linearPos;
            fishingLine.SetPosition(1, bait.position);

            time += Time.deltaTime / duration;
            yield return null;
        }

        bait.position = end;
        fishingLine.SetPosition(1, bait.position);
    }

    private IEnumerator CheckBiteAfterDelay(float delay)
    {
        waitingForBite = true;
        yield return new WaitForSeconds(delay);

        float catchChance = Random.Range(0f, 1f);

        if (catchChance > 0.3f)
        {
            Debug.Log("Fish On! Press 'R' to reel in.");
            ShowFishingStatus("Fish On!");
            fishOnHook = true;
            reelingTimer = 0f;
            isReeling = false;
        }
        else
        {
            Debug.Log("Nothing took the bait. Reel it back in and try again.");
            ShowFishingStatus("Nothing took the bait.");
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
        {
            StartCoroutine(ReelBaitBack(currentBait.transform));
        }

        if (reelingCountdownText != null && allowableCountdownText != null)
        {
            reelingCountdownText.gameObject.SetActive(false);
            allowableCountdownText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ReelBaitBack(Transform bait)
    {
        Vector3 start = bait.position;
        Vector3 end = rodTip.position;
        float duration = 0.5f;
        float time = 0f;

        while (time < duration)
        {
            bait.position = Vector3.Lerp(start, end, time / duration);
            fishingLine.SetPosition(1, bait.position);
            time += Time.deltaTime;
            yield return null;
        }

        fishingLine.SetPosition(1, end);
        Destroy(bait.gameObject);
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
            caughtItem = "Bass";
        }
        else if (catchResult < 67)
        {
            caughtItem = "Trout";
        }
        else
        {
            caughtItem = "Boot";
        }

        Debug.Log("You caught a " + caughtItem + "!");

        if (caughtFishText != null)
        {
            caughtFishText.text = "You caught a " + caughtItem + "!";
            caughtFishText.gameObject.SetActive(true);
            caughtFishTimer = 0f;
        }

        FindFirstObjectByType<ChecklistHandler>().MarkItemCaught(caughtItem);
        FindFirstObjectByType<JournalHandler>().CompleteAchievement("Caught a " + caughtItem);


        totalFishCaught++;
        uniqueFishCaught.Add(caughtItem);

        if (totalFishCaught == 5)
        {
            FindFirstObjectByType<JournalHandler>().CompleteAchievement("Fished 5 times");
        }

        if (uniqueFishCaught.Contains("Bass") && uniqueFishCaught.Contains("Trout") && uniqueFishCaught.Contains("Boot"))
        {
            FindFirstObjectByType<JournalHandler>().CompleteAchievement("Caught All Fish Types");
        }
    }

    private void ShowFishingStatus(string message)
    {
        if (fishingStatusText != null)
        {
            fishingStatusText.text = message;
            fishingStatusText.gameObject.SetActive(true);
            statusTimer = 0f;
        }
    }

    public void ChangePlayerAnimationState(string newAnimationState)
    {
        if (animationStateChanger != null)
        {
            animationStateChanger.ChangeAnimationState(newAnimationState);
        }
    }

}
