using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : Menu
{
    [Header("Main Areas")]
    [SerializeField] GameObject mainScreenMainArea = default;
    [Header("Confirmation Messages")]
    [SerializeField] [TextArea(3, 5)] string newGameWarning = default;
    [SerializeField] [TextArea(3, 5)] string quitWarning = default;
    [Header("Other References")]
    [SerializeField] TextMeshProUGUI appVersionText = default;

    protected override void Start()
    {
        base.Start();

        appVersionText.text = "Version " + Application.version;
        
        GameManager.Instance.SetCursorEnable(enable: true);  
        GameManager.Instance.ConfirmationPrompt.AddCancelationListener(delegate { CancelConfirmation(); });
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
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(newGameWarning);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void ShowQuitConfirmation()
    {
        mainScreenMainArea.SetActive(false);
        GameManager.Instance.ConfirmationPrompt.AddConfirmationListener(delegate { QuitGame(); });
        GameManager.Instance.ConfirmationPrompt.ChangeWarningMessage(quitWarning);
        GameManager.Instance.ConfirmationPrompt.ShowConfirmation();
    }

    public void CancelConfirmation()
    {
        mainScreenMainArea.SetActive(true);
        GameManager.Instance.ConfirmationPrompt.RemoveAllConfirmationListeners();
    }
}