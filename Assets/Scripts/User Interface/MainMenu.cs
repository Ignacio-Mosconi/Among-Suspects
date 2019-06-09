using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : Menu
{
    [SerializeField] GameObject mainScreenMainArea = default;
    [Header("COnfirmation Messages")]
    [SerializeField] [TextArea(3, 5)] string newGameWarning = default;
    [SerializeField] [TextArea(3, 5)] string quitWarning = default;
    ConfirmationPrompt confirmationPrompt;

    void Awake()
    {
        confirmationPrompt = mainScreen.GetComponentInChildren<ConfirmationPrompt>(includeInactive: true);
    }

    protected override void Start()
    {
        base.Start();
        
        GameManager.Instance.SetCursorEnable(enable: true);  
        confirmationPrompt.AddCancelationListener(delegate { CancelConfirmation(); });
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
        confirmationPrompt.AddConfirmationListener(delegate { StartNewGame(); });
        confirmationPrompt.ChangeWarningMessage(newGameWarning);
        confirmationPrompt.ShowConfirmation();
    }

    public void ShowQuitConfirmation()
    {
        mainScreenMainArea.SetActive(false);
        confirmationPrompt.AddConfirmationListener(delegate { QuitGame(); });
        confirmationPrompt.ChangeWarningMessage(quitWarning);
        confirmationPrompt.ShowConfirmation();
    }

    public void CancelConfirmation()
    {
        mainScreenMainArea.SetActive(true);
        confirmationPrompt.RemoveAllConfirmationListeners();
    }
}