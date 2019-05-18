using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    #region Singleton

    static ChapterManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static ChapterManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<ChapterManager>();
                if (!instance)
                    Debug.LogError("There is no 'Chapter Manager' in the scene");
            }

            return instance;
        }
    }

    #endregion

    [SerializeField] GameObject endScreenArea;
    [SerializeField] GameObject debateRetryArea;
    [SerializeField] GameObject chapterWonArea;
    [SerializeField] ClueInfo[] chapterClues;

    DebateInitializer debateInitializer;

    void Start()
    {
        debateInitializer = FindObjectOfType<DebateInitializer>();
    }

    public ClueInfo GetChapterClueInfo(int index)
    {
        ClueInfo clueInfo = null;

        if (index < chapterClues.Length && index >= 0)
            clueInfo = chapterClues[index];

        return clueInfo;
    }

    public void ShowDebateEndScreen(bool hasWon)
    {
        if (hasWon)
            chapterWonArea.SetActive(true);
        else
            debateRetryArea.SetActive(true);
        
        endScreenArea.SetActive(true);
        GameManager.Instance.SetCursorAvailability(enable: true);
    }

    public void RetryDebate()
    {
        endScreenArea.SetActive(false);
        debateRetryArea.SetActive(false);
        debateInitializer.StartDebate();
    }
}