using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class StartMenuHandler : MonoBehaviour
{
    public GameObject optionsUI;                // Assign the Options UI parent object
    public TMP_Dropdown resolutionDropdown;     // Assign the resolution dropdown
    public Slider volumeSlider;                 // Assign the volume slider
    public AudioMixer audioMixer;               // Link your AudioMixer asset

    private Resolution[] resolutions;

    void Start()
    {
        if (optionsUI != null)
            optionsUI.SetActive(false);  // Hide options UI on start

        SetupResolutions();
    }

    void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("FirstWorld");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenOptions()
    {
        if (optionsUI != null)
            optionsUI.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsUI != null)
            optionsUI.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20);
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
