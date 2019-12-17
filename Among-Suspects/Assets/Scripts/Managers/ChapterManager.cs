using System;
using UnityEngine;
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

    [Header("Main Area")]
    [SerializeField] GameObject endScreenArea = default;
    [Header("Animated Screens")]
    [SerializeField] AnimatedMenuScreen debateResultsScreen = default;
    [SerializeField] AnimatedMenuScreen debateRetryScreen = default;
    [Header("Various Controllers")]
    [SerializeField] ObjectStateController objectStateController = default;
    [Header("Confirmation Prompt Messages")]
    [SerializeField] [TextArea(3, 5)] string[] debateStartWarnings = new string[(int)Language.Count];
    [SerializeField] [TextArea(3, 5)] string[] exitDebateWarnings = new string[(int)Language.Count];
    [SerializeField] [TextArea(3, 5)] string[] nextChapterWarnings = new string[(int)Language.Count];

    ClueInfo[] chapterClues;
    DebateInitializer debateInitializer;
    PauseMenu pauseMenu;
    ChapterPhase currentPhase = ChapterPhase.Exploration;

    void Start()
    {
        debateInitializer = FindObjectOfType<DebateInitializer>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        objectStateController.SetExplorationVariableMeshesMaterials();

        LoadChapterClues();

        debateResultsScreen.SetUp();
        debateRetryScreen.SetUp();

        GameManager.Instance.SetCursorEnable(enable: false);
        AudioManager.Instance.PostEvent("Inicio_Juego");
        AudioManager.Instance.PostEvent("Lluvia_Interior");

        GameManager.Instance.AddCursorPointerEventsToAllButtons(endScreenArea);
        GameManager.Instance.OnLanguageChanged.AddListener(LoadChapterClues);

        debateInitializer.DisableInteraction();
    }

    void LoadChapterClues()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        string languagePath = Enum.GetName(typeof(Language), language);
        
        chapterClues = Resources.LoadAll<ClueInfo>("Clues/" + languagePath + "/" + SceneManager.GetActiveScene().name);

        CharacterManager.Instance.PlayerController.ReloadCluesGathered(chapterClues);
    }

    void RemoveAllConfirmationPromptListeners()
    {
        GameManager.Instance.ConfirmationPrompt.RemoveAllConfirmationListeners();
        GameManager.Instance.ConfirmationPrompt.RemoveAllCancelationListeners();
    }

    void ConfirmDebateStart()
    {
        SetPauseAvailability(enable: true);
        RemoveAllConfirmationPromptListeners();
        AudioManager.Instance.PostEvent("Pase_Modo_Debate");
        debateInitializer.StartDebate();
    }

    void CancelDebateStart()
    {
        SetPauseAvailability(enable: true);
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
        SetPauseAvailability(enable: false);

        if (hasWon)
            debateResultsScreen.Show();
        else
            debateRetryScreen.Show();
        
        endScreenArea.SetActive(true);
        GameManager.Instance.SetCursorEnable(enable: true);
    }

    public void ShowDebateStartConfirmation()
    {
        SetPauseAvailability(enable: false);

        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { ConfirmDebateStart(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelDebateStart(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(debateStartWarnings[(int)GameManager.Instance.CurrentLanguage]);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void ShowExitConfirmation()
    {
        endScreenArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { ExitGame(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelExit(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(exitDebateWarnings[(int)GameManager.Instance.CurrentLanguage]);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void ShowNextChapterConfirmation()
    {
        endScreenArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { ExitGame(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelExit(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(nextChapterWarnings[(int)GameManager.Instance.CurrentLanguage]);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void RetryDebate()
    {
        SetPauseAvailability(enable: true);

        endScreenArea.SetActive(false);
        debateRetryScreen.Hide();
        debateInitializer.StartDebate();
    }

    public void TriggerInvestigationPhase()
    {
        if (currentPhase == ChapterPhase.Exploration)
        {
            currentPhase = ChapterPhase.Investigation;
            objectStateController.SetInvestigationVariableMeshesMaterials();
            debateInitializer.EnableInteraction();
            CharacterManager.Instance.LoadDialogues(ChapterPhase.Investigation);
            CharacterManager.Instance.PlayerController.StartInvestigation();
            AudioManager.Instance.PostEvent("Descubrir_Cadaver");
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        GameManager gameManager = GameManager.Instance;
        string mainMenuSceneName = gameManager.GetMainMenuSceneName();
        AudioManager.Instance.PostEvent("Pausa_Off");
        gameManager.TransitionToScene(mainMenuSceneName);
    }

    public void SetPauseAvailability(bool enable)
    {
        pauseMenu.enabled = enable;
    }

    public ClueInfo GetChapterClueInfo(int index)
    {
        ClueInfo clueInfo = null;

        if (index < chapterClues.Length && index >= 0)
            clueInfo = chapterClues[index];

        return clueInfo;
    }

    public ClueInfo GetChapterClueInfo(uint clueID)
    {
        ClueInfo clueInfo = Array.Find(chapterClues, ci => ci.clueID == clueID);

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