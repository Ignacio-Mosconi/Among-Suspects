using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : Menu
{
    [SerializeField] GameObject menuArea = default;

    ConfirmationPrompt exitConfirmationPrompt;
    bool isPaused;
    bool playerMovementEnabledAtPause;
    bool cursorEnabledAtPause;

    UnityEvent onPaused = new UnityEvent();
    UnityEvent onResume = new UnityEvent();

    void Awake()
    {
        exitConfirmationPrompt = GetComponentInChildren<ConfirmationPrompt>(includeInactive: true);
    }

    protected override void Start()
    {
        base.Start();

        exitConfirmationPrompt.AddConfirmationListener(delegate { ExitGame(); });
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    void ExitGame()
    {
        Time.timeScale = 1f;

        GameManager gameManager = GameManager.Instance;
        string mainMenuSceneName = gameManager.GetMainMenuSceneName();
        gameManager.TransitionToScene(mainMenuSceneName);
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        menuArea.SetActive(true);

        playerMovementEnabledAtPause = CharacterManager.Instance.PlayerController.IsMovementEnabled();
        cursorEnabledAtPause = GameManager.Instance.IsCursorEnabled();

        CharacterManager.Instance.PlayerController.Disable();
        DialogueManager.Instance.SetUpdateEnable(enable: false);   
        DebateManager.Instance.SetUpdateEnable(enable: false);
        GameManager.Instance.SetCursorEnable(enable: true);
        
        onPaused.Invoke();
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        menuArea.SetActive(false);

        ResetMenuState();

        if (playerMovementEnabledAtPause)
            CharacterManager.Instance.PlayerController.Enable();
        DialogueManager.Instance.SetUpdateEnable(enable: true);
        DebateManager.Instance.SetUpdateEnable(enable: true);
        GameManager.Instance.SetCursorEnable(enable: cursorEnabledAtPause);
        
        onResume.Invoke();
    }

    public void ShowExitConfirmation()
    {
        exitConfirmationPrompt.ShowConfirmation();
    }

    #region Properties

    public UnityEvent OnPaused
    {
        get { return onPaused; }
    }

    public UnityEvent OnResume
    {
        get { return onResume; }
    }

    #endregion
}