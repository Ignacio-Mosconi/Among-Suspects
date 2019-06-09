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
    [Header("Confirmation Messages")]
    [SerializeField] [TextArea(3, 5)] string debateStartWarning = default;
    [SerializeField] [TextArea(3, 5)] string quitWarning = default;

    ClueInfo[] chapterClues;
    ConfirmationPrompt confirmationPrompt;
    DebateInitializer debateInitializer;
    PauseMenu pauseMenu;
    ChapterPhase currentPhase = ChapterPhase.Exploration;

    void Start()
    {
        confirmationPrompt = GetComponentInChildren<ConfirmationPrompt>(includeInactive: true);
        chapterClues = Resources.LoadAll<ClueInfo>("Clues/" + SceneManager.GetActiveScene().name);
        debateInitializer = FindObjectOfType<DebateInitializer>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        GameManager.Instance.SetCursorEnable(enable: false);

        debateInitializer.DisableInteraction();
    }

    void ConfirmDebateStart()
    {
        pauseMenu.enabled = true;
        debateInitializer.StartDebate();
    }

    void ExitGame()
    {
        Time.timeScale = 1f;

        GameManager gameManager = GameManager.Instance;
        string mainMenuSceneName = gameManager.GetMainMenuSceneName();
        gameManager.TransitionToScene(mainMenuSceneName);
    }

    void CancelDebateStart()
    {
        pauseMenu.enabled = true;

        confirmationPrompt.RemoveAllConfirmationListeners();
        confirmationPrompt.RemoveAllCancelationListeners();
        debateInitializer.CancelDebate();
    }

    void CancelExit()
    {
        if (pauseMenu.enabled)
            pauseMenu.ActivateMenuArea();
        else
            endScreenArea.SetActive(true);

        confirmationPrompt.RemoveAllConfirmationListeners();
        confirmationPrompt.RemoveAllCancelationListeners();
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

        confirmationPrompt.AddConfirmationListener(delegate { ConfirmDebateStart(); });
        confirmationPrompt.AddCancelationListener(delegate { CancelDebateStart(); });
        confirmationPrompt.ChangeWarningMessage(debateStartWarning);
        confirmationPrompt.ShowConfirmation();
    }

    public void ShowExitConfirmation()
    {
        endScreenArea.SetActive(false);
        confirmationPrompt.AddConfirmationListener(delegate { ExitGame(); });
        confirmationPrompt.AddCancelationListener(delegate { CancelExit(); });
        confirmationPrompt.ChangeWarningMessage(quitWarning);
        confirmationPrompt.ShowConfirmation();
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
        }
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