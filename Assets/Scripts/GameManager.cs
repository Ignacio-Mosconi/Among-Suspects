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

    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
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

    #endregion
}