using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : Menu
{
    [SerializeField] GameObject menuArea = default;

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
                Resume();
        }
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
        
        onResume.Invoke();
    }

    public void ShowExitConfirmation()
    {
        menuArea.SetActive(false);
        ChapterManager.Instance.ShowExitConfirmation();
    }

    public void ActivateMenuArea()
    {
        menuArea.SetActive(true);
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