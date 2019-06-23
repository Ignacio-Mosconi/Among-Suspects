using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

    [Header("Main Areas")]
    [SerializeField] GameObject endScreenArea = default;
    [SerializeField] GameObject debateRetryArea = default;
    [SerializeField] GameObject chapterWonArea = default;
    [Header("Confirmation Prompt Messages")]
    [SerializeField] [TextArea(3, 5)] string debateStartWarning = default;
    [SerializeField] [TextArea(3, 5)] string exitDebateWarning = default;
    [SerializeField] [TextArea(3, 5)] string nextChapterWarning = default;

    ClueInfo[] chapterClues;
    DebateInitializer debateInitializer;
    PauseMenu pauseMenu;
    ChapterPhase currentPhase = ChapterPhase.Exploration;

    void Start()
    {
        chapterClues = Resources.LoadAll<ClueInfo>("Clues/" + SceneManager.GetActiveScene().name);
        debateInitializer = FindObjectOfType<DebateInitializer>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        GameManager.Instance.SetCursorEnable(enable: false);
        AudioManager.Instance.PlayAmbientSound("Rain Interior");
        AudioManager.Instance.PlayTheme("Exploration Phase");

        GameManager.Instance.AddCursorPointerEventsToAllButtons(endScreenArea);

        debateInitializer.DisableInteraction();
    }

    void RemoveAllConfirmationPromptListeners()
    {
        GameManager.Instance.ConfirmationPrompt.RemoveAllConfirmationListeners();
        GameManager.Instance.ConfirmationPrompt.RemoveAllCancelationListeners();
    }

    void ConfirmDebateStart()
    {
        pauseMenu.enabled = true;
        RemoveAllConfirmationPromptListeners();
        AudioManager.Instance.PlayTheme("Debate");
        debateInitializer.StartDebate();
    }

    void CancelDebateStart()
    {
        pauseMenu.enabled = true;
        RemoveAllConfirmationPromptListeners();
        debateInitializer.CancelDebate();
    }

    void CancelExit()
    {
        endScreenArea.SetActive(true);
        RemoveAllConfirmationPromptListeners();
    }

    public void ShowDebateEndScreen(bool hasWon)
    {
        pauseMenu.enabled = false;

        if (hasWon)
            chapterWonArea.SetActive(true);
        else
            debateRetryArea.SetActive(true);
        
        endScreenArea.SetActive(true);
        GameManager.Instance.SetCursorEnable(enable: true);
    }

    public void ShowDebateStartConfirmation()
    {
        pauseMenu.enabled = false;

        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { ConfirmDebateStart(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelDebateStart(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(debateStartWarning);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void ShowExitConfirmation()
    {
        endScreenArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { ExitGame(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelExit(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(exitDebateWarning);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void ShowNextChapterConfirmation()
    {
        endScreenArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { ExitGame(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelExit(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(nextChapterWarning);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
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
            debateInitializer.EnableInteraction();
            CharacterManager.Instance.LoadDialogues(ChapterPhase.Investigation);
            CharacterManager.Instance.PlayerController.StartInvestigation();
            AudioManager.Instance.PlayTheme("Investigation Phase");
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        GameManager gameManager = GameManager.Instance;
        string mainMenuSceneName = gameManager.GetMainMenuSceneName();
        gameManager.TransitionToScene(mainMenuSceneName);
    }

    public ClueInfo GetChapterClueInfo(int index)
    {
        ClueInfo clueInfo = null;

        if (index < chapterClues.Length && index >= 0)
            clueInfo = chapterClues[index];

        return clueInfo;
    }

    #region Properties
    
    public ChapterPhase CurrentPhase
    {
        get { return currentPhase; }
    }

    public int CluesAmount
    {
        get { return chapterClues.Length; }
    }

    #endregion
}