using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct MouseCursor
{
    public Texture2D texture;
    public Vector2 hotspot;
}

public class GameManager : MonoBehaviour
{
    #region Singleton

    static GameManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(transform.parent.gameObject);
        else
        {
            DontDestroyOnLoad(transform.parent.gameObject);
            AwakeSetUp();
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<GameManager>();
                if (!instance)
                {
                    GameObject gameManagerPrefab = Resources.Load("Game Management/Game Manager Canvas") as GameObject;
                    instance = Instantiate(gameManagerPrefab).GetComponentInChildren<GameManager>();
                }
            }

            return instance;
        }
    }

    #endregion

    [Header("Application's Default Settings")]
    [SerializeField] [Range(24, 60)] int targetFrameRate = 60;
    [SerializeField] Color playerSpeakingTextColor = default;
    [SerializeField] Color playerThinkingTextColor = default;
    [SerializeField] Color npcSpeakingTextColor = default;
    [SerializeField] Color tutorialTextColor = default;

    [Header("Scenes")]
    [SerializeField] string mainMenuSceneName = default;
    [SerializeField] string[] chapterScenesNames = default;
    [Header("Mouse Cursors")]
    [SerializeField] MouseCursor normalCursor = default;
    [SerializeField] MouseCursor selectionCursor = default;

    public const float MinLodingTime = 3f;
    public const float MinFullscreenDPI = 90f;

    ScreenFader screenFader;
    LoadingScreen loadingScreen;
    ConfirmationPrompt confirmationPrompt;
    Resolution[] availableResolutions;
    int currentQualityLevelIndex;
    int currentResolutionIndex;
    bool isFullscreen;
    float textSpeedMultiplier;
    float charactersShowIntervals;
    float sfxVolume;
    float musicVolume;

    void AwakeSetUp()
    {
        screenFader = GetComponentInChildren<ScreenFader>();
        loadingScreen = GetComponentInChildren<LoadingScreen>(includeInactive: true);
        confirmationPrompt = GetComponentInChildren<ConfirmationPrompt>(includeInactive: true);
    }

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;     
        FetchAvailableResolutions();
        LoadPlayerPrefs(); 
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void SetBoolPreference(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }

    bool GetBoolPreference(string key, bool defaultValue)
    {
        int defaultIntValue = defaultValue ? 1 : 0;
        int value = PlayerPrefs.GetInt(key, defaultIntValue);
        return (value == 1);
    }

    void FetchAvailableResolutions()
    {
        availableResolutions = Screen.resolutions;
        Array.Reverse(availableResolutions);
    }

    void LoadPlayerPrefs()
    {
        Resolution currentRes = Screen.currentResolution;
        int defaultResIndex = Array.FindIndex(availableResolutions, r => r.width == currentRes.width && r.height == currentRes.height);

        currentQualityLevelIndex = PlayerPrefs.GetInt("pQualityLevel", QualitySettings.GetQualityLevel());
        currentResolutionIndex = PlayerPrefs.GetInt("pResolution", defaultResIndex);
        isFullscreen = GetBoolPreference("pFullscreen", Screen.fullScreen);
        textSpeedMultiplier = PlayerPrefs.GetFloat("pTextSpeedMultiplier", 1f);
        sfxVolume = PlayerPrefs.GetFloat("pSfxVolume", 0.75f);
        musicVolume = PlayerPrefs.GetFloat("pMusicVolume", 0.75f);

        SetQualityLevel(currentQualityLevelIndex);
        SetResolution(currentResolutionIndex);
        SetFullscreen(isFullscreen);
        SetTextSpeed(textSpeedMultiplier);
        SetSfxVolume(sfxVolume);
        SetMusicVolume(musicVolume);
    }

    void OnMousePointerEnter(PointerEventData data, Selectable selectable)
    {     
        Button button = selectable as Button;
        if (button && button.interactable)
        {
            Cursor.SetCursor(selectionCursor.texture, selectionCursor.hotspot, CursorMode.Auto);
            AudioManager.Instance.PlaySound("Button Highlight");
        }
    }

    void OnMousePointerExit(PointerEventData data, Selectable selectable)
    {
        Cursor.SetCursor(normalCursor.texture, normalCursor.hotspot, CursorMode.Auto);
    }

    void OnMousePointerClick(PointerEventData data, Selectable selectable)
    {
        Button button = selectable as Button;
        if (button && button.image.sprite != button.spriteState.disabledSprite)
        {
            Cursor.SetCursor(normalCursor.texture, normalCursor.hotspot, CursorMode.Auto);
            if (!AudioManager.Instance.IsPlayingSound("Button Click"))
                AudioManager.Instance.PlaySound("Button Click");
        }      
    }

    void AddCursorPointerEvent(Selectable selectable, EventTriggerType triggerType)
    {
        EventTrigger trigger = selectable.GetComponent<EventTrigger>();
        if (!trigger)
            trigger = selectable.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = triggerType;

        switch (triggerType)
        {
            case EventTriggerType.PointerEnter:
                entry.callback.AddListener((data) => { OnMousePointerEnter((PointerEventData)data, selectable); });
                break;
            case EventTriggerType.PointerExit:
                entry.callback.AddListener((data) => { OnMousePointerExit((PointerEventData)data, selectable); });
                break;
            case EventTriggerType.PointerClick:
                entry.callback.AddListener((data) => { OnMousePointerClick((PointerEventData)data, selectable); });
                break;
        }

        trigger.triggers.Add(entry);
    }

    void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsynchronously(sceneName));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        screenFader.FadeInScene();
    }

    IEnumerator InvokeRealTime(Action action, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        action();
    }

    IEnumerator InvokeScaledTime<T>(Action<T> action, T parameter, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        action(parameter);
    }

    IEnumerator InvokeRealTime<T>(Action<T> action, T paramenter, float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        action(paramenter);
    }

    IEnumerator LoadSceneAsynchronously(string sceneName)
    {
        float loadingTimer = 0f;
        float currentProgress = 0f;
        float maxProgressValue = 0.0f + MinLodingTime;
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        loadOperation.allowSceneActivation = false;
        loadingScreen.Show();

        while (!loadOperation.isDone)
        {
            loadingTimer += Time.deltaTime;
            currentProgress = Mathf.Clamp01((loadOperation.progress + loadingTimer) / maxProgressValue);

            loadingScreen.ChangeLoadPercentage(currentProgress);

            if (currentProgress == 1f)
                loadOperation.allowSceneActivation = true;

            yield return new WaitForEndOfFrame();
        }

        loadingScreen.Hide();
    }

    public void SetCursorEnable(bool enable)
    {   
        Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
    }

    public void TransitionToScene(string sceneName)
    {
        SetCursorEnable(enable: false);
        confirmationPrompt.RemoveAllConfirmationListeners();
        confirmationPrompt.RemoveAllCancelationListeners();
        screenFader.FadeOutScene();
        InvokeMethodInRealTime(LoadNextScene, sceneName, screenFader.FadeDuration);
    }

    public void AddCursorPointerEventsToAllButtons(GameObject uiLayout)
    {
        Button[] buttons = uiLayout.GetComponentsInChildren<Button>(includeInactive: true);
        foreach (Button button in buttons)
        {
            AddCursorPointerEvent(button, EventTriggerType.PointerEnter);
            AddCursorPointerEvent(button, EventTriggerType.PointerExit);
            AddCursorPointerEvent(button, EventTriggerType.PointerClick);
        }
    }

    public void AddCursorPointerEvents(Button button)
    {
        AddCursorPointerEvent(button, EventTriggerType.PointerEnter);
        AddCursorPointerEvent(button, EventTriggerType.PointerExit);
        AddCursorPointerEvent(button, EventTriggerType.PointerClick);
    }

    public void InvokeMethodInScaledTime<T>(Action<T> action, T parameter, float waitTime)
    {
        StartCoroutine(InvokeScaledTime(action, parameter, waitTime));
    }

    public void InvokeMethodInRealTime(Action action, float waitTime)
    {
        StartCoroutine(InvokeRealTime(action, waitTime));
    }
    
    public void InvokeMethodInRealTime<T>(Action<T> action, T parameter, float waitTime)
    {
        StartCoroutine(InvokeRealTime(action, parameter, waitTime));
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public bool IsCursorEnabled()
    {
        return (Cursor.visible);
    }

    public string GetMainMenuSceneName()
    {
        return mainMenuSceneName;
    }

    public string GetChapterSceneName(uint chapterIndex)
    {
        if (chapterIndex >= chapterScenesNames.Length)
        {
            Debug.LogError("There is no chapter with that index");
            return null;
        }

        return chapterScenesNames[chapterIndex];
    }

    public bool ShouldBeWindowed()
    {
        float maxScreenWidth = availableResolutions[0].width;
        float maxScreenHeight = availableResolutions[0].height;
        float currentScreenWidth = availableResolutions[currentResolutionIndex].width;
        float currentScreenHeight = availableResolutions[currentResolutionIndex].height;
        float screenSize = Mathf.Sqrt(maxScreenWidth * maxScreenWidth + maxScreenHeight * maxScreenHeight) / Screen.dpi; 
        float currentDPI = Mathf.Sqrt(currentScreenWidth * currentScreenWidth + currentScreenHeight * currentScreenHeight) / screenSize;

        return (currentDPI < MinFullscreenDPI); 
    }

    public bool ShouldBeFullscreen()
    {
        return (currentResolutionIndex == 0); 
    }

    public void SetQualityLevel(int qualityLevelIndex)
    {
        currentQualityLevelIndex = qualityLevelIndex;
        PlayerPrefs.SetInt("pQualityLevel", currentQualityLevelIndex);
        
        QualitySettings.SetQualityLevel(qualityLevelIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        currentResolutionIndex = resolutionIndex;
        PlayerPrefs.SetInt("pResolution", currentResolutionIndex);

        Resolution res = availableResolutions[currentResolutionIndex];
        Screen.SetResolution(res.width, res.height, isFullscreen);
    }

    public void SetFullscreen(bool fullScreen)
    {
        isFullscreen = fullScreen;
        SetBoolPreference("pFullscreen", isFullscreen);
        
        Screen.fullScreen = isFullscreen;
    }

    public void SetTextSpeed(float speedMultiplier)
    {
        textSpeedMultiplier = speedMultiplier;
        PlayerPrefs.SetFloat("pTextSpeedMultiplier", textSpeedMultiplier);

        charactersShowIntervals = 1f / (textSpeedMultiplier * targetFrameRate);
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("pSfxVolume", sfxVolume);

        AudioManager.Instance.SetMixerVolume(MixerType.Sfx, sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        PlayerPrefs.SetFloat("pMusicVolume", musicVolume);

        AudioManager.Instance.SetMixerVolume(MixerType.Music, musicVolume);
    }

    #region Properties

    public ConfirmationPrompt ConfirmationPrompt
    {
        get { return confirmationPrompt; }
    }

    public Resolution[] AvailableResolutions
    {
        get { return availableResolutions; }
    }

    public int CurrentQualityLevelIndex
    {
        get { return currentQualityLevelIndex; }
    }

    public int CurrentResolutionIndex
    {
        get { return currentResolutionIndex; }
    }

    public bool IsFullscreen
    {
        get { return isFullscreen; }
    }

    public float TextSpeedMultiplier
    {
        get { return textSpeedMultiplier; }
    }

    public float SfxVolume
    {
        get { return sfxVolume; }
    }

    public float MusicVolume
    {
        get { return musicVolume; }
    }
    
    public float CharactersShowIntervals
    {
        get { return charactersShowIntervals; }
    }

    public Color PlayerSpeakingTextColor
    {
        get { return playerSpeakingTextColor; }
    }

    public Color PlayerThinkingTextColor
    {
        get { return playerThinkingTextColor; }
    }

    public Color NpcSpeakingTextColor
    {
        get { return npcSpeakingTextColor; }
    }

    public Color TutorialTextColor
    {
        get { return tutorialTextColor; }
    }

    #endregion
}