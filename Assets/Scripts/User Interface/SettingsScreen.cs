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
    [Header("Info Panel")]
    [SerializeField] RectTransform infoPanel = default;
    [SerializeField] [Range(0f, 100f)] float infoPanelAppearHorOffset = 75f;
    [SerializeField] [TextArea(3, 10)] string fullscreenInfoText = default;
    [SerializeField] [TextArea(3, 10)] string textSpeedInfoText = default;
    [Header("Other Properties")]
    [SerializeField] Image sfxAudioIcon = default;
    [SerializeField] Image musicAudioIcon = default;
    [SerializeField] Sprite[] audioIconsSprites = default;
    [SerializeField] [Range(150f, 300f)] float maxDropdownHeight = 300f;
    
    TextMeshProUGUI infoPanelText;
    TextMeshProUGUI sfxVolumeValueText;
    TextMeshProUGUI musicVolumeValueText;

    const float InfoPanelBorderPadding = 20f;

    void Awake()
    {
        infoPanelText = infoPanel.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
        sfxVolumeValueText = sfxVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
        musicVolumeValueText = musicVolumeSlider.GetComponentInChildren<TextMeshProUGUI>();
    }
    
    void Start()
    {
        InitializeQualityLevelDropdown();
        InitializeResolutionDropdown();
        fullscreenToggle.isOn = GameManager.Instance.IsFullscreen;
        textSpeedSlider.value = GameManager.Instance.TextSpeedMultiplier;
        sfxVolumeSlider.value = GameManager.Instance.SfxVolume;
        musicVolumeSlider.value = GameManager.Instance.MusicVolume;
        sfxAudioIcon.sprite = (sfxVolumeSlider.value != 0f) ? audioIconsSprites[0] : audioIconsSprites[1];
        musicAudioIcon.sprite = (musicVolumeSlider.value != 0f) ? audioIconsSprites[0] : audioIconsSprites[1];
    }

    void OnEnable()
    {
        HideInfoPanel();
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

    void ResizeInfoPanel()
    {
        infoPanelText.ForceMeshUpdate();

        float newPanelHeight = infoPanelText.renderedHeight + InfoPanelBorderPadding;
        infoPanel.sizeDelta = new Vector2(infoPanel.sizeDelta.x, newPanelHeight);

        Vector2 offset = new Vector2(infoPanelAppearHorOffset, -infoPanel.rect.height);
        infoPanel.anchoredPosition = (Vector2)Input.mousePosition + offset;
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
        sfxAudioIcon.sprite = (volume != 0f) ? audioIconsSprites[0] : audioIconsSprites[1];
        sfxVolumeValueText.text = ((int)(volume * 100f)).ToString();
        GameManager.Instance.SetSfxVolume(volume);
        AudioManager.Instance.PlaySound("Button Click", oneShot: false);
    }

    public void SetSfxMute()
    {
        float newVolume = (sfxAudioIcon.sprite == audioIconsSprites[0]) ? 0f : 1f;
        sfxVolumeSlider.value = newVolume;
    }

    public void ChangeMusicVolume(float volume)
    {
        musicAudioIcon.sprite = (volume != 0f) ? audioIconsSprites[0] : audioIconsSprites[1];
        musicVolumeValueText.text = ((int)(volume * 100f)).ToString();
        GameManager.Instance.SetMusicVolume(volume);
    }

    public void SetMusicMute()
    {
        float newVolume = (musicAudioIcon.sprite == audioIconsSprites[0]) ? 0f : 1f;
        musicVolumeSlider.value = newVolume;
    }

    public void ShowFullscreenInfo()
    {
        infoPanel.gameObject.SetActive(true);    
        infoPanelText.text = fullscreenInfoText;
        ResizeInfoPanel(); 
    }

    public void ShowTextSpeedInfo()
    {
        infoPanel.gameObject.SetActive(true);    
        infoPanelText.text = textSpeedInfoText;
        ResizeInfoPanel();
    }

    public void HideInfoPanel()
    {
        infoPanel.gameObject.SetActive(false);
    }
}