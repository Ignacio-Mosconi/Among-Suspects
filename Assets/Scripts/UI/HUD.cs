using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject hudArea = default;
    [SerializeField] GameObject interactTextPanel = default;
    [SerializeField] GameObject clueFoundPrompt = default;
    [SerializeField] GameObject investigationPhasePrompt = default;
    [SerializeField] [Range(1.5f, 2.5f)] float promptsDuration = 2f;

    void Start()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();

        foreach (Interactable interactable in interactables)
        {
            interactable.OnStartLookingAt.AddListener(ShowInteractTextPanel);
            interactable.OnStopLookingAt.AddListener(HideInteractTextPanel);
            interactable.OnInteraction.AddListener(HideInteractTextPanel);
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

    void ShowInteractTextPanel()
    {
        interactTextPanel.SetActive(true);
    }

    void HideInteractTextPanel()
    {
        interactTextPanel.SetActive(false);
    }

    void ShowClueFoundPrompt()
    {
        clueFoundPrompt.SetActive(true);
        Invoke("HideClueFoundPrompt", promptsDuration);
    }

    void HideClueFoundPrompt()
    {
        clueFoundPrompt.SetActive(false);
    }

    void ShowInvestigationPhasePrompt()
    {
        investigationPhasePrompt.SetActive(true);
        Invoke("HideInvestigationPhasePrompt", promptsDuration);
    }

    void HideInvestigationPhasePrompt()
    {
        investigationPhasePrompt.SetActive(false);
    }
}