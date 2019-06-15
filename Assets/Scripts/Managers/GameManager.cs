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
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
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
                    GameObject gameManagerPrefab = Resources.Load("Game Management/Game Manager") as GameObject;
                    instance = Instantiate(gameManagerPrefab).GetComponent<GameManager>();
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

    float charactersShowIntervals;

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        charactersShowIntervals = 1f / (textSpeedMultiplier * targetFrameRate);
    }

    void OnMousePointerEnter(PointerEventData data)
    {
        Cursor.SetCursor(selectionCursor.texture, selectionCursor.hotspot, CursorMode.Auto);
    }

    void OnMousePointerExit(PointerEventData data)
    {
        Cursor.SetCursor(normalCursor.texture, normalCursor.hotspot, CursorMode.Auto);
    }

    void AddCursorPointerEvent(GameObject uiElement, EventTriggerType triggerType)
    {
        EventTrigger trigger = uiElement.GetComponent<EventTrigger>();
        if (!trigger)
            trigger = uiElement.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = triggerType;

        switch (triggerType)
        {
            case EventTriggerType.PointerEnter:
                entry.callback.AddListener((data) => { OnMousePointerEnter((PointerEventData)data); });
                break;
            case EventTriggerType.PointerExit:
            case EventTriggerType.PointerClick:
                entry.callback.AddListener((data) => { OnMousePointerExit((PointerEventData)data); });
                break;
        }

        trigger.triggers.Add(entry);
    }

    public void SetCursorEnable(bool enable)
    {   
        Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
    }

    public void TransitionToScene(string sceneName)
    {
        SetCursorEnable(enable: false);
        SceneManager.LoadScene(sceneName);
    }

    public void AddCursorPointerEventsToAllButtons(GameObject uiLayout)
    {
        Button[] buttons = uiLayout.GetComponentsInChildren<Button>(includeInactive: true);
        foreach (Button button in buttons)
        {
            AddCursorPointerEvent(button.gameObject, EventTriggerType.PointerEnter);
            AddCursorPointerEvent(button.gameObject, EventTriggerType.PointerExit);
            AddCursorPointerEvent(button.gameObject, EventTriggerType.PointerClick);
        }
    }

    public void AddCursorPointerEvents(Button button)
    {
        AddCursorPointerEvent(button.gameObject, EventTriggerType.PointerEnter);
        AddCursorPointerEvent(button.gameObject, EventTriggerType.PointerExit);
        AddCursorPointerEvent(button.gameObject, EventTriggerType.PointerClick);
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