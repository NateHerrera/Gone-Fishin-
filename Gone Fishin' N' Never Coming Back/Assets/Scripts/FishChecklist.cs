using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ChecklistHandler : MonoBehaviour
{
    public GameObject togglePrefab;
    public Transform contentParent;

    private Dictionary<string, Toggle> checklistToggles = new Dictionary<string, Toggle>();

    void Start()
    {
        AddChecklistItem("Trout");
        AddChecklistItem("Bass");
        AddChecklistItem("Boot");
    }

    void AddChecklistItem(string itemName)
    {
        GameObject newToggle = Instantiate(togglePrefab, contentParent);
        Toggle toggleComponent = newToggle.GetComponent<Toggle>();
        toggleComponent.isOn = false;  // Start off

        TextMeshProUGUI label = newToggle.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = itemName;

        checklistToggles[itemName] = toggleComponent;
    }

    public void MarkItemCaught(string itemName)
    {
        if (checklistToggles.ContainsKey(itemName))
        {
            checklistToggles[itemName].isOn = true;
        }
    }
}
