using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject hudArea = default;
    [SerializeField] UIPrompt interactTextPrompt = default;
    [SerializeField] UIPrompt clueFoundPrompt = default;
    [SerializeField] UIPrompt investigationPhasePrompt = default;
    [SerializeField] string interactionKeyString = default;

    TextMeshProUGUI interactText;

    void Start()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();

        foreach (Interactable interactable in interactables)
        {
            interactable.OnStartLookingAt.AddListener(ShowInteractTextPrompt);
            interactable.OnStopLookingAt.AddListener(HideInteractTextPrompt);
            interactable.OnInteraction.AddListener(DeactivateInteractTextPrompt);
        }

        TutorialController tutorialController = FindObjectOfType<TutorialController>();
        if (tutorialController)
            tutorialController.OnTutorialTriggered.AddListener(HideHUD);

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.OnPaused.AddListener(HideHUD);
        pauseMenu.OnResume.AddListener(ShowHUD);

        DialogueManager.Instance.OnDialogueAreaEnable.AddListener(HideHUD);
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(ShowHUD);
        
        PlayerController playerController = CharacterManager.Instance.PlayerController;
        playerController.OnClueFound.AddListener(ShowClueFoundPrompt);
        playerController.OnStartedInvestigation.AddListener(ShowInvestigationPhasePrompt);

        interactTextPrompt.SetUp();
        clueFoundPrompt.SetUp();
        investigationPhasePrompt.SetUp();

        interactText = interactTextPrompt.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
    }

    void ShowHUD()
    {
        if (!DialogueManager.Instance.enabled)
            hudArea.SetActive(true);
    }

    void HideHUD()
    {
        hudArea.SetActive(false);
    }

    void ShowInteractTextPrompt(string interactionKind)
    {
        interactText.text = "Press " + interactionKeyString + " to <color=yellow>" + interactionKind + "</color>";
        interactTextPrompt.Show();
    }

    void HideInteractTextPrompt()
    {
        interactTextPrompt.Hide();
    }

    void DeactivateInteractTextPrompt()
    {
        interactTextPrompt.Deactivate();
    }

    void ShowClueFoundPrompt()
    {
        clueFoundPrompt.Show();
        float promptDur = clueFoundPrompt.GetOnScreenDuration();
        CharacterManager.Instance.PlayerController.Invoke("ReEnableInteractionDelayed", promptDur);
    }

    void ShowInvestigationPhasePrompt()
    {
        investigationPhasePrompt.Show();
        float promptDur = investigationPhasePrompt.GetOnScreenDuration();
        CharacterManager.Instance.PlayerController.Invoke("ReEnableInteractionDelayed", promptDur);
    }

    #region Properties

    public UIPrompt InvestigationPhasePrompt
    {
        get { return investigationPhasePrompt; }
    }

    public UIPrompt ClueFoundPrompt
    {
        get { return clueFoundPrompt; }
    }

    #endregion
}