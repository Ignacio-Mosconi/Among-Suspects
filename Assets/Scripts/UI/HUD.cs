using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject hudArea;
    [SerializeField] GameObject interactTextPanel;
    [SerializeField] GameObject debateStartPrompt;

    DebateController debateController;

    void Start()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();

        foreach (Interactable interactable in interactables)
        {
            interactable.OnStartLookingAt.AddListener(ShowInteractTextPanel);
            interactable.OnStopLookingAt.AddListener(HideInteractTextPanel);
            interactable.OnInteraction.AddListener(HideInteractTextPanel);
        }

        debateController = FindObjectOfType<DebateController>();
        debateController.OnInteraction.AddListener(ShowDebateStartPrompt);

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.OnPaused.AddListener(HideHUD);
        pauseMenu.OnResume.AddListener(ShowHUD);

        DialogueManager.Instance.OnDialogueAreaEnable.AddListener(HideHUD);
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(ShowHUD);
    }

    void ShowHUD()
    {
        hudArea.SetActive(true);
    }

    void HideHUD()
    {
        hudArea.SetActive(false);
    }

    void ShowInteractTextPanel()
    {
        interactTextPanel.SetActive(true);
    }

    void HideInteractTextPanel()
    {
        interactTextPanel.SetActive(false);
    }

    void ShowDebateStartPrompt()
    {
        debateStartPrompt.SetActive(true);
    }

    void HideDebateStartPrompt()
    {
        debateStartPrompt.SetActive(false);
    }

    public void ConfirmDebateStart()
    {
        HideDebateStartPrompt();
        debateController.StartDebate();
    }

    public void CancelDebateStart()
    {
        HideDebateStartPrompt();
        debateController.CancelDebate();
    }
}