using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] Dropdown qualityLevelDropdown = default;
    [SerializeField] Dropdown resolutionDropdown = default;
    [SerializeField] Toggle fullscreenToggle = default;
    [SerializeField] Slider textSpeedSlider = default;
    [SerializeField] Slider sfxVolumeSlider = default;
    [SerializeField] Slider musicVolumeSlider = default;
    
    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        InitializeQualityLevelDropdown();
        InitializeResolutionDropdown();
    }

    void InitializeQualityLevelDropdown()
    {
        List<string> qualityLevelsStrings = new List<string>(QualitySettings.names);

        qualityLevelDropdown.ClearOptions();
        qualityLevelDropdown.AddOptions(qualityLevelsStrings);
        qualityLevelDropdown.value = GameManager.Instance.CurrentQualityLevelIndex;
    }

    void InitializeResolutionDropdown()
    {
        Resolution[] resolutions = Screen.resolutions.Select(resolution => new Resolution
        {
            width = resolution.width,
            height = resolution.height
        }).Distinct().ToArray();

        List<string> resolutionsStrings = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionsStrings.Add(option);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionsStrings);
        resolutionDropdown.value = GameManager.Instance.CurrentResolutionIndex;
    }

    public void ChangeQualityLevel(int qualityLevelIndex)
    {
        GameManager.Instance.SetQualityLevel(qualityLevelIndex);
    }

    public void ChangeResolution(int resolutionIndex)
    {
        GameManager.Instance.SetResolution(resolutionIndex);
    }

    public void ChangeFullscreen(bool fullscreen)
    {
        GameManager.Instance.SetFullscreen(fullscreen);
    }
}