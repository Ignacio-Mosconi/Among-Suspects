using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject hudArea = default;
    [SerializeField] GameObject interactTextPrompt = default;
    [SerializeField] UIPrompt clueFoundPrompt = default;
    [SerializeField] UIPrompt investigationPhasePrompt = default;

    void Start()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();

        foreach (Interactable interactable in interactables)
        {
            interactable.OnStartLookingAt.AddListener(ShowInteractTextPrompt);
            interactable.OnStopLookingAt.AddListener(HideInteractTextPrompt);
            interactable.OnInteraction.AddListener(HideInteractTextPrompt);
        }

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.OnPaused.AddListener(HideHUD);
        pauseMenu.OnResume.AddListener(ShowHUD);

        DialogueManager.Instance.OnDialogueAreaEnable.AddListener(HideHUD);
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(ShowHUD);
        
        PlayerController playerController = CharacterManager.Instance.PlayerController;
        playerController.OnClueFound.AddListener(ShowClueFoundPrompt);
        playerController.OnStartedInvestigation.AddListener(ShowInvestigationPhasePrompt);
    }

    void ShowHUD()
    {
        hudArea.SetActive(true);
    }

    void HideHUD()
    {
        hudArea.SetActive(false);
    }

    void ShowInteractTextPrompt()
    {
        interactTextPrompt.SetActive(true);
    }

    void HideInteractTextPrompt()
    {
        interactTextPrompt.SetActive(false);
    }

    void ShowClueFoundPrompt()
    {
        clueFoundPrompt.Display();
    }

    void ShowInvestigationPhasePrompt()
    {
        investigationPhasePrompt.Display();
    }
}