using UnityEngine;
using TMPro;

public class MainMenu : Menu
{
    [Header("Main Areas")]
    [SerializeField] GameObject mainScreenMainArea = default;
    [Header("Confirmation Messages")]
    [SerializeField] [TextArea(3, 5)] string[] newGameWarnings = new string[(int)Language.Count];
    [SerializeField] [TextArea(3, 5)] string[] quitWarnings = new string[(int)Language.Count];
    [Header("Other References")]
    [SerializeField] TextMeshProUGUI appVersionText = default;
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI[] backButtonTexts = default; 
    [SerializeField] TextMeshProUGUI[] mainButtonTexts = default; 
    [SerializeField] TextMeshProUGUI controlsTitleText = default; 
    [SerializeField] TextMeshProUGUI[] controlsTexts = default;
    [SerializeField] TextMeshProUGUI settingsTitleText = default;
    [SerializeField] TextMeshProUGUI[] settingsOptionsTexts = default;
    [SerializeField] TextMeshProUGUI[] textSpeedTexts = default;
    [SerializeField] TextMeshProUGUI creditsTitleText = default;
    [SerializeField] TextMeshProUGUI thanksText = default;

    protected override void Start()
    {
        base.Start();

        appVersionText.text = "Version " + Application.version;
        
        GameManager.Instance.SetCursorEnable(enable: true);
        AudioManager.Instance.PlayTheme("Main Menu");
    }

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        MainMenuTextInfo mainMenuTextInfo = menuTextsByLanguage[language] as MainMenuTextInfo;

        if (!mainMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        for (int i = 0; i < backButtonTexts.Length; i++)
            backButtonTexts[i].text = mainMenuTextInfo.backButtonText;
        for (int i = 0; i < mainButtonTexts.Length; i++)
            mainButtonTexts[i].text = mainMenuTextInfo.mainButtonTexts[i];
        controlsTitleText.text = mainMenuTextInfo.controlsTitle;
        for (int i = 0; i < controlsTexts.Length; i++)
            controlsTexts[i].text = mainMenuTextInfo.controlsTexts[i];
        settingsTitleText.text = mainMenuTextInfo.settingsTitle;
        for (int i = 0; i < settingsOptionsTexts.Length; i++)
            settingsOptionsTexts[i].text = mainMenuTextInfo.settingsOptions[i];
        for (int i = 0; i < textSpeedTexts.Length; i++)
            textSpeedTexts[i].text = mainMenuTextInfo.textSpeedTexts[i];
        creditsTitleText.text = mainMenuTextInfo.creditsTitle;
        thanksText.text = mainMenuTextInfo.thanksText;
    }

    void StartNewGame()
    {
        GameManager gameManager = GameManager.Instance;
        string firstChapterName = gameManager.GetChapterSceneName(0);  
        gameManager.TransitionToScene(firstChapterName);
    }

    void QuitGame()
    {
        GameManager.Instance.QuitApplication();
    }

    public void ShowNewGameConfirmation()
    {
        mainScreenMainArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { StartNewGame(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelConfirmation(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(newGameWarnings[(int)GameManager.Instance.CurrentLanguage]);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void ShowQuitConfirmation()
    {
        mainScreenMainArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { QuitGame(); });
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelConfirmation(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(quitWarnings[(int)GameManager.Instance.CurrentLanguage]);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void CancelConfirmation()
    {
        mainScreenMainArea.SetActive(true);
        GameManager.Instance.ConfirmationPrompt.RemoveAllConfirmationListeners();
        GameManager.Instance.ConfirmationPrompt.RemoveAllCancelationListeners();
    }
}