using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : Menu
{
    [Header("Warning Messages")]
    [SerializeField] [TextArea(3, 5)] string newGameWarning = default;
    [SerializeField] [TextArea(3, 5)] string quitWarning = default;
    ConfirmationPrompt genericConfirmationPrompt;

    void Awake()
    {
        genericConfirmationPrompt = mainScreen.GetComponentInChildren<ConfirmationPrompt>(includeInactive: true);
    }

    protected override void Start()
    {
        base.Start();
        
        GameManager.Instance.SetCursorEnable(enable: true);  
        genericConfirmationPrompt.AddCancelationListener(delegate { CancelConfirmation(); });
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
        genericConfirmationPrompt.AddConfirmationListener(delegate { StartNewGame(); });
        genericConfirmationPrompt.ChangeWarningMessage(newGameWarning);
        genericConfirmationPrompt.ShowConfirmation();
    }

    public void ShowQuitConfirmation()
    {
        genericConfirmationPrompt.AddConfirmationListener(delegate { QuitGame(); });
        genericConfirmationPrompt.ChangeWarningMessage(quitWarning);
        genericConfirmationPrompt.ShowConfirmation();
    }

    public void CancelConfirmation()
    {
        genericConfirmationPrompt.RemoveAllConfirmationListeners();
    }
}