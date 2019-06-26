using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : Menu
{
    [SerializeField] GameObject menuArea = default;
    [SerializeField] GameObject mainScreenMainArea = default;
    [SerializeField] [TextArea(3, 5)] string exitWarning = default;

    bool isPaused;
    bool playerMovementEnabledAtPause;
    bool cursorEnabledAtPause;

    UnityEvent onPaused = new UnityEvent();
    UnityEvent onResume = new UnityEvent();

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!isPaused)
                Pause();
            else
            {
                AudioManager.Instance.PlaySound("Menu Pop Out");
                Resume();
            }
        }
    }

    void CancelExit()
    {
        mainScreenMainArea.SetActive(true);
        GameManager.Instance.ConfirmationPrompt.RemoveAllConfirmationListeners();
        GameManager.Instance.ConfirmationPrompt.RemoveAllCancelationListeners();
    }

    void ExitGame()
    {
        ChapterManager.Instance.ExitGame();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        menuArea.SetActive(true);

        playerMovementEnabledAtPause = CharacterManager.Instance.PlayerController.IsMovementEnabled();
        cursorEnabledAtPause = GameManager.Instance.IsCursorEnabled();

        CharacterManager.Instance.PlayerController.Disable();
        DialogueManager.Instance.PauseUpdate(); 
        DebateManager.Instance.SetUpdateEnable(enable: false);
        GameManager.Instance.SetCursorEnable(enable: true);

        AudioManager.Instance.PauseAmbientSound();
        AudioManager.Instance.PlaySound("Menu Pop In");

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
        DialogueManager.Instance.ResumeUpdate();
        DebateManager.Instance.SetUpdateEnable(enable: true);
        GameManager.Instance.SetCursorEnable(enable: cursorEnabledAtPause);

        AudioManager.Instance.ResumeAmbientSound();

        onResume.Invoke();
    }

    public void ShowExitConfirmation()
    {
        mainScreenMainArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { ExitGame(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelExit(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(exitWarning);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
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