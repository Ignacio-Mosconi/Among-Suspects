using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : Menu
{
    [SerializeField] GameObject menuArea = default;

    PlayerController playerController;
    bool isPaused;

    bool playerMovementEnabledAtPause;
    bool cursorEnabledAtPause;

    UnityEvent onPaused = new UnityEvent();
    UnityEvent onResume = new UnityEvent();

    protected override void Start()
    {
        base.Start();
        playerController = FindObjectOfType<PlayerController>();
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

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        menuArea.SetActive(true);

        playerMovementEnabledAtPause = playerController.IsMovementAvailable();
        cursorEnabledAtPause = GameManager.Instance.IsCursorAvailable();

        playerController.SetAvailability(enable: false);
        DialogueManager.Instance.SetUpdateEnable(enable: false);   
        DebateManager.Instance.SetUpdateEnable(enable: false);
        GameManager.Instance.SetCursorAvailability(enable: true);
        
        onPaused.Invoke();
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        menuArea.SetActive(false);

        ResetMenuState();
        
        playerController.SetAvailability(enable: playerMovementEnabledAtPause);
        DialogueManager.Instance.SetUpdateEnable(enable: true);
        DebateManager.Instance.SetUpdateEnable(enable: true);
        GameManager.Instance.SetCursorAvailability(enable: cursorEnabledAtPause);
        
        onResume.Invoke();
    }

    #region Getters & Setters

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