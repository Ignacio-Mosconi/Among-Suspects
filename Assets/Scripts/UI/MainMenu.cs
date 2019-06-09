using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : Menu
{
    [Header("Confirmation Message")]
    [SerializeField] GameObject confirmationArea = default;
    [SerializeField] Button confirmButton = default;
    [SerializeField] Button cancelButton = default;
    [SerializeField] TextMeshProUGUI warningText = default;
    [SerializeField] [TextArea(3, 5)] string newGameWarning = default;
    [SerializeField] [TextArea(3, 5)] string quitWarning = default;

    string firstChapterName;

    protected override void Start()
    {
        base.Start();
        
        GameManager.Instance.SetCursorEnable(enable: true);  
        firstChapterName = GameManager.Instance.GetChapterSceneName(0);
        cancelButton.onClick.AddListener(() => CancelConfirmation());
    }

    public void ShowNewGameConfirmation()
    {
        confirmButton.onClick.AddListener(() => StartNewGame());
        warningText.text = newGameWarning;
        confirmationArea.SetActive(true);
    }

    public void ShowQuitConfirmation()
    {
        confirmButton.onClick.AddListener(() => QuitGame());
        warningText.text = quitWarning;
        confirmationArea.SetActive(true);
    }

    public void CancelConfirmation()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmationArea.SetActive(false);
    }

    public void StartNewGame()
    {
        GameManager.Instance.TransitionToScene(firstChapterName);
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitApplication();
    }
}