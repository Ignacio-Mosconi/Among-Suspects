using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PauseMenu : Menu
{
    [Header("Main Properties")]
    [SerializeField] GameObject menuArea = default;
    [SerializeField] GameObject mainScreenMainArea = default;
    [SerializeField] [TextArea(3, 5)] string[] exitWarnings = new string[(int)Language.Count];

    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI[] backButtonTexts = default;
    [SerializeField] TextMeshProUGUI pauseTitleText = default;
    [SerializeField] TextMeshProUGUI[] mainButtonTexts = default;
    [SerializeField] TextMeshProUGUI cluesTitleText = default;
    [SerializeField] TextMeshProUGUI inventoryTitleText = default;
    [SerializeField] TextMeshProUGUI settingsTitleText = default;
    [SerializeField] TextMeshProUGUI[] settingsOptionsTexts = default;
    [SerializeField] TextMeshProUGUI[] textSpeedTexts = default;

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

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        PauseMenuTextInfo pauseMenuTextInfo = menuTextsByLanguage[language] as PauseMenuTextInfo;

        if (!pauseMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        for (int i = 0; i < backButtonTexts.Length; i++)
            backButtonTexts[i].text = pauseMenuTextInfo.backButtonText;
        pauseTitleText.text = pauseMenuTextInfo.pauseTitle;
        for (int i = 0; i < mainButtonTexts.Length; i++)
            mainButtonTexts[i].text = pauseMenuTextInfo.mainButtonTexts[i];
        cluesTitleText.text = pauseMenuTextInfo.cluesTitle;
        inventoryTitleText.text = pauseMenuTextInfo.inventoryTitle;;
        settingsTitleText.text = pauseMenuTextInfo.settingsTitle;
        for (int i = 0; i < settingsOptionsTexts.Length; i++)
            settingsOptionsTexts[i].text = pauseMenuTextInfo.settingsOptions[i];
        for (int i = 0; i < textSpeedTexts.Length; i++)
            textSpeedTexts[i].text = pauseMenuTextInfo.textSpeedTexts[i];
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
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(exitWarnings[(int)GameManager.Instance.CurrentLanguage]);
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