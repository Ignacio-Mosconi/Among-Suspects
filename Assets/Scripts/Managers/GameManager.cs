using System;
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
    [SerializeField] [Range(0.5f, 2f)] float textSpeedMultiplier = 1f;
    [SerializeField] Color playerSpeakingTextColor = default;
    [SerializeField] Color playerThinkingTextColor = default;
    [SerializeField] Color npcSpeakingTextColor = default;

    [Header("Scenes")]
    [SerializeField] string mainMenuSceneName = default;
    [SerializeField] string[] chapterScenesNames = default;
    [Header("Mouse Cursors")]
    [SerializeField] MouseCursor normalCursor = default;
    [SerializeField] MouseCursor selectionCursor = default;

    const float MinLodingTime = 3f;

    ScreenFader screenFader;
    LoadingScreen loadingScreen;
    ConfirmationPrompt confirmationPrompt;
    float charactersShowIntervals;

    void AwakeSetUp()
    {
        screenFader = GetComponentInChildren<ScreenFader>();
        loadingScreen = GetComponentInChildren<LoadingScreen>(includeInactive: true);
        confirmationPrompt = GetComponentInChildren<ConfirmationPrompt>(includeInactive: true);
    }

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        charactersShowIntervals = 1f / (textSpeedMultiplier * targetFrameRate);

        SceneManager.sceneLoaded += OnSceneLoaded;
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

    #region Properties
    
    public ConfirmationPrompt ConfirmationPrompt
    {
        get { return confirmationPrompt; }
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

    #endregion
}