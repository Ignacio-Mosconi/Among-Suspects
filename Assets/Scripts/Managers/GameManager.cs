using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    static GameManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
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

    [SerializeField] [Range(24, 60)] int targetFrameRate = 60;
    [SerializeField] [Range(0.5f, 2f)] float textSpeedMultiplier = 1f;
    [SerializeField] Color playerSpeakingTextColor;
    [SerializeField] Color playerThinkingTextColor;
    [SerializeField] Color npcSpeakingTextColor;

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;

        SetCursorAvailability(enable: false);
    }

    public void SetCursorAvailability(bool enable)
    {   
        Cursor.lockState = (enable) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
    }

    #region Getters & Setters
    
    public float TargetFrameRate
    {
        get { return targetFrameRate; }
    }

    public float TextSpeedMultiplier
    {
        get { return textSpeedMultiplier; }
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