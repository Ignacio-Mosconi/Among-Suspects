using System;
using UnityEngine;
using UnityEngine.Events;

public class TutorialController : MonoBehaviour
{   
    [SerializeField] TutorialTrigger debateTutorialTrigger = default;

    TutorialInfo[] tutorials;
    float navigationTutorialDelay;
    float investigationTutorialDelay;
    float cluesTutorialDelay;
    float debateStartTutorialDelay;

    const float AdditionalTutorialDelay = 0.1f;

    UnityEvent onTutorialTriggered = new UnityEvent();

    void Start()
    {
        tutorials = Resources.LoadAll<TutorialInfo>("Tutorials");

        HUD hud = FindObjectOfType<HUD>();

        navigationTutorialDelay = DialogueManager.Instance.SpeechPanelPrompt.HideAnimationDuration + AdditionalTutorialDelay;
        investigationTutorialDelay = hud.InvestigationPhasePrompt.GetOnScreenDuration() + AdditionalTutorialDelay;
        cluesTutorialDelay = hud.ClueFoundPrompt.GetOnScreenDuration() + AdditionalTutorialDelay;
        debateStartTutorialDelay = cluesTutorialDelay;
        
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(TriggerNavigationTutorial);
        CharacterManager.Instance.PlayerController.OnStartedInvestigation.AddListener(TriggerInvestigationTutorial);
        CharacterManager.Instance.PlayerController.OnClueFound.AddListener(TriggerCluesTutorial);
        CharacterManager.Instance.PlayerController.OnAllCluesFound.AddListener(TriggerDebateStartTutorial);

        debateTutorialTrigger.gameObject.SetActive(false);
        debateTutorialTrigger.OnTrigger.AddListener(DisplayDebateTutorial);
    }

    TutorialInfo FetchTutorial(TutorialType tutorialType)
    {
        TutorialInfo tutorial = Array.Find(tutorials, t => t.tutorialType == tutorialType);
        if (!tutorial)
            Debug.LogError("There are no '" + tutorialType.ToString() + "' tutorials set up");

        return tutorial;
    }

    void DisplayTutorial(TutorialInfo tutorial)
    {
        DialogueManager.Instance.StartDialogue(tutorial);
        onTutorialTriggered.Invoke();
    }

    void TriggerNavigationTutorial()
    {
        Invoke("DisplayNavigationTutorial", navigationTutorialDelay);
    }

    void TriggerInvestigationTutorial()
    {
        Invoke("DisplayInvestigationTutorial", investigationTutorialDelay);
    }

    void TriggerCluesTutorial()
    {
        Invoke("DisplayCluesTutorial", cluesTutorialDelay);
    }

    void TriggerDebateStartTutorial()
    {
        debateTutorialTrigger.gameObject.SetActive(true);
        Invoke("DisplayDebateStartTutorial", debateStartTutorialDelay);
    }

    void DisplayNavigationTutorial()
    {
        TutorialInfo tutorial = FetchTutorial(TutorialType.Navigation);
        DialogueManager.Instance.OnDialogueAreaDisable.RemoveListener(TriggerNavigationTutorial);
        DisplayTutorial(tutorial);
    }

    void DisplayInvestigationTutorial()
    {
        TutorialInfo tutorial = FetchTutorial(TutorialType.Investigation);
        CharacterManager.Instance.PlayerController.OnStartedInvestigation.RemoveListener(TriggerInvestigationTutorial);
        DisplayTutorial(tutorial);
    }

    void DisplayCluesTutorial()
    {
        TutorialInfo tutorial = FetchTutorial(TutorialType.Clues);
        CharacterManager.Instance.PlayerController.OnClueFound.RemoveListener(TriggerCluesTutorial);
        DisplayTutorial(tutorial);
    }

    void DisplayDebateStartTutorial()
    {
        TutorialInfo tutorial = FetchTutorial(TutorialType.DebateStart);
        CharacterManager.Instance.PlayerController.OnAllCluesFound.RemoveListener(TriggerDebateStartTutorial);
        DisplayTutorial(tutorial);
    }

    void DisplayDebateTutorial()
    {
        TutorialInfo tutorial = FetchTutorial(TutorialType.Debate);
        debateTutorialTrigger.OnTrigger.RemoveListener(DisplayDebateTutorial);
        DisplayTutorial(tutorial);
    }

    #region Properties

    public UnityEvent OnTutorialTriggered
    {
        get { return onTutorialTriggered; }
    }

    #endregion
}