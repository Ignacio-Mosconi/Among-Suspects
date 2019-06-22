using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsScreen : MonoBehaviour
{
    [Header("Dropdowns")]
    [SerializeField] TMP_Dropdown qualityLevelDropdown = default;
    [SerializeField] TMP_Dropdown resolutionDropdown = default;
    [Header("Toggles")]
    [SerializeField] Toggle fullscreenToggle = default;
    [Header("Sliders")]
    [SerializeField] Slider textSpeedSlider = default;
    [SerializeField] Slider sfxVolumeSlider = default;
    [SerializeField] Slider musicVolumeSlider = default;
    [Header("Texts")]
    [SerializeField] TextMeshProUGUI fullscreenText = default;
    [Header("Other Properties")]
    [SerializeField] [Range(150f, 300f)] float maxDropdownHeight = 300f;
    
    void Start()
    {
        InitializeQualityLevelDropdown();
        InitializeResolutionDropdown();
        fullscreenToggle.isOn = GameManager.Instance.IsFullscreen;
        textSpeedSlider.value = GameManager.Instance.TextSpeedMultiplier;
        sfxVolumeSlider.value = GameManager.Instance.SfxVolume;
        musicVolumeSlider.value = GameManager.Instance.MusicVolume;
    }

    void ResizeDropdownList(TMP_Dropdown dropdown)
    {
        RectTransform templateTransform = dropdown.template;
        RectTransform viewportTransform = templateTransform.GetComponentInChildren<Mask>().rectTransform;
        RectTransform contentTransform = viewportTransform.GetChild(0).GetComponent<RectTransform>();
        float contentHeight = contentTransform.sizeDelta.y;
        int numberOfOptions = dropdown.options.Count;
        float newTemplateHeight = Mathf.Min(contentHeight * numberOfOptions, maxDropdownHeight);

        templateTransform.sizeDelta = new Vector2(templateTransform.sizeDelta.x, newTemplateHeight);
    }

    void InitializeQualityLevelDropdown()
    {
        List<string> qualityLevelsStrings = new List<string>(QualitySettings.names);

        qualityLevelDropdown.ClearOptions();
        qualityLevelDropdown.AddOptions(qualityLevelsStrings);
        qualityLevelDropdown.value = GameManager.Instance.CurrentQualityLevelIndex;

        ResizeDropdownList(qualityLevelDropdown);
    }

    void InitializeResolutionDropdown()
    {
        Resolution[] resolutions = GameManager.Instance.AvailableResolutions;

        List<string> resolutionsStrings = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resolutionsStrings.Add(option);
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionsStrings);
        resolutionDropdown.value = GameManager.Instance.CurrentResolutionIndex;
        ValidateResolution();

        ResizeDropdownList(resolutionDropdown);
    }

    void ValidateResolution()
    {
        bool shouldBeWindowed = GameManager.Instance.ShouldBeWindowed();
        bool shouldBeFullscreen = GameManager.Instance.ShouldBeFullscreen();

        if (shouldBeWindowed || shouldBeFullscreen)
        {
            fullscreenToggle.interactable = false;
            fullscreenToggle.graphic.color = fullscreenToggle.colors.disabledColor;
            fullscreenText.color = Color.grey;
            fullscreenToggle.isOn = shouldBeFullscreen;
            ChangeFullscreen(shouldBeFullscreen);
        }
        else
            if (!fullscreenToggle.interactable)
        {
            fullscreenToggle.interactable = true;
            fullscreenToggle.graphic.color = fullscreenToggle.colors.normalColor;
            fullscreenText.color = Color.white;
        }
    }

    public void ChangeQualityLevel(int qualityLevelIndex)
    {
        GameManager.Instance.SetQualityLevel(qualityLevelIndex);
    }

    public void ChangeResolution(int resolutionIndex)
    {
        GameManager.Instance.SetResolution(resolutionIndex);
        ValidateResolution();
    }

    public void ChangeFullscreen(bool fullscreen)
    {
        GameManager.Instance.SetFullscreen(fullscreen);
    }

    public void ChangeTextSpeed(float speedMultiplier)
    {
        GameManager.Instance.SetTextSpeed(speedMultiplier);
    }

    public void ChangeSfxVolume(float volume)
    {
        GameManager.Instance.SetSfxVolume(volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        GameManager.Instance.SetMusicVolume(volume);
    }
}