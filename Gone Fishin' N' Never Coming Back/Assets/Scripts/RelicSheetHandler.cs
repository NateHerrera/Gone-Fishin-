using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class JournalHandler : MonoBehaviour
{
    public GameObject togglePrefab;          // Assign your Toggle prefab
    public Transform contentParent;          // Assign the Vertical Layout Group
    private Dictionary<string, Toggle> journalToggles = new Dictionary<string, Toggle>();
    public GameObject nameplatePrefab;  // Assign a prefab with a TextMeshPro
    public Transform nameplateSpawnPoint; // Assign the empty transform over the player's head

    void Start()
    {
        AddAchievement("Caught a Bass");
        AddAchievement("Caught a Trout");
        AddAchievement("Caught a Boot");
        AddAchievement("Fished 5 times");
        AddAchievement("Caught All Fish Types");
        AddAchievement("Find The Pool!");
        

        // Optional: Hide panel on start
        if (contentParent != null)
            contentParent.gameObject.SetActive(false);
    }

    void AddAchievement(string achievementName)
    {
        GameObject newToggle = Instantiate(togglePrefab, contentParent);
        Toggle toggle = newToggle.GetComponent<Toggle>();
        toggle.isOn = false;  // Initially unchecked

        // Hide the checkmark until completed
        toggle.graphic.gameObject.SetActive(false);

        TextMeshProUGUI label = newToggle.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = achievementName;

        journalToggles[achievementName] = toggle;
    }

    void Update()
    {
        // Close the sheet panel when Escape is pressed
        if (contentParent.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            contentParent.gameObject.SetActive(false);
        }
    }

    public void CompleteAchievement(string achievementName)
    {
        if (journalToggles.ContainsKey(achievementName))
        {
            Toggle toggle = journalToggles[achievementName];
            toggle.isOn = true;
            toggle.graphic.gameObject.SetActive(true); // Show checkmark
        }
    }

    public void ToggleSheetPanel()
    {
        if (contentParent != null)
        {
            contentParent.gameObject.SetActive(!contentParent.gameObject.activeSelf);
        }
    }

    public void EquipAchievementNameplate(string labelText)
    {
        foreach (Transform child in nameplateSpawnPoint)
        {
            Destroy(child.gameObject); // Clear old plate
        }

        GameObject plate = Instantiate(nameplatePrefab, nameplateSpawnPoint);
        plate.GetComponentInChildren<TextMeshProUGUI>().text = labelText;
    }
}
