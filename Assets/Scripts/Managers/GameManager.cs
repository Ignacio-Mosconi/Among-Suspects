using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] SceneAsset mainMenuScene = default;
    [SerializeField] SceneAsset[] chapterScenes = default;

    float charactersShowIntervals;

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        charactersShowIntervals = 1f / (textSpeedMultiplier * targetFrameRate);
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
        return mainMenuScene.name;
    }

    public string GetChapterSceneName(uint chapterIndex)
    {
        if (chapterIndex >= chapterScenes.Length)
        {
            Debug.LogError("There is no chapter with that index");
            return null;
        }

        return chapterScenes[chapterIndex].name;
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