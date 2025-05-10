using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;

public class PauseMenuHandler : MonoBehaviour
{
    public GameObject pauseMenuCanvas;  
    public GameObject optionsPanel;      
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public Toggle fullscreenToggle;

    private bool isPaused = false;
    private Resolution[] resolutions;

    void Awake()
    {
        int resIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        Resolution[] allRes = Screen.resolutions;
        if (resIndex >= 0 && resIndex < allRes.Length)
        {
            Resolution res = allRes[resIndex];
            Screen.SetResolution(res.width, res.height, isFullscreen);
        }
    }

    void Start()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        Load();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }

    public void ToggleOptionsPanel()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volumeSlider.value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetResolution(int resolutionIndex)
    {
        // Store selected index, actual resolution applied only on Apply
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // Store setting, apply later
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void ApplyGraphicsSettings()
    {
        int resolutionIndex = resolutionDropdown.value;
        bool isFullscreen = fullscreenToggle.isOn;

        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, isFullscreen);

        Save(); // Save preferences after applying
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume");
            SetVolume(volumeSlider.value);
        }

        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex");
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        }
    }
}
