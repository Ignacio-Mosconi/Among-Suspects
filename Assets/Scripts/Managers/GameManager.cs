using UnityEngine;

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
                    Debug.LogError("There is no 'GameManager' in the scene");
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

    float charactersShowIntervals;

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        charactersShowIntervals = 1f / (textSpeedMultiplier * targetFrameRate);

        SetCursorEnable(enable: false);
    }

    public void SetCursorEnable(bool enable)
    {   
        Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
    }

    public bool IsCursorEnabled()
    {
        return (Cursor.visible);
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