using UnityEngine;

public enum ChapterPhase
{
    Exploration, Investigation
}

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
    [SerializeField] GameObject debateStartPromptArea;
    [SerializeField] GameObject debateRetryArea;
    [SerializeField] GameObject chapterWonArea;
    [SerializeField] ClueInfo[] chapterClues;

    DebateInitializer debateInitializer;
    PauseMenu pauseMenu;
    ChapterPhase currentPhase = ChapterPhase.Exploration;

    void Start()
    {
        debateInitializer = FindObjectOfType<DebateInitializer>();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    public ClueInfo GetChapterClueInfo(int index)
    {
        ClueInfo clueInfo = null;

        if (index < chapterClues.Length && index >= 0)
            clueInfo = chapterClues[index];

        return clueInfo;
    }

    void HideDebateStartPrompt()
    {
        pauseMenu.enabled = true;
        debateStartPromptArea.SetActive(false);
    }

    public void ShowDebateStartPrompt()
    {
        pauseMenu.enabled = false;
        debateStartPromptArea.SetActive(true);
    }

    public void ConfirmDebateStart()
    {
        HideDebateStartPrompt();
        debateInitializer.StartDebate();
    }

    public void CancelDebateStart()
    {
        HideDebateStartPrompt();
        debateInitializer.CancelDebate();
    }

    public void ShowDebateEndScreen(bool hasWon)
    {
        pauseMenu.enabled = false;

        if (hasWon)
            chapterWonArea.SetActive(true);
        else
            debateRetryArea.SetActive(true);
        
        endScreenArea.SetActive(true);
        GameManager.Instance.SetCursorAvailability(enable: true);
    }

    public void RetryDebate()
    {
        pauseMenu.enabled = true;

        endScreenArea.SetActive(false);
        debateRetryArea.SetActive(false);
        debateInitializer.StartDebate();
    }

    public void TriggerInvestigationPhase()
    {
        if (currentPhase == ChapterPhase.Exploration)
        {
            currentPhase = ChapterPhase.Investigation;
            CharacterManager.Instance.LoadInvestigationDialogues();
        }
    }

    public ChapterPhase CurrentPhase
    {
        get { return currentPhase; }
    }
}