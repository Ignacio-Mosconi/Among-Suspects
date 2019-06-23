using System;
using UnityEngine;
using UnityEngine.Events;

public class TutorialController : MonoBehaviour
{   
    TutorialInfo[] tutorials;
    float navigationTutorialDelay;
    float investigationTutorialDelay;
    float cluesTutorialDelay;

    const float AdditionalTutorialDelay = 0.1f;

    UnityEvent onTutorialTriggered = new UnityEvent();

    void Start()
    {
        tutorials = Resources.LoadAll<TutorialInfo>("Tutorials");

        HUD hud = FindObjectOfType<HUD>();

        navigationTutorialDelay = DialogueManager.Instance.SpeechPanelPrompt.HideAnimationDuration + AdditionalTutorialDelay;
        investigationTutorialDelay = hud.InvestigationPhasePrompt.GetOnScreenDuration() + AdditionalTutorialDelay;
        cluesTutorialDelay = hud.ClueFoundPrompt.GetOnScreenDuration() + AdditionalTutorialDelay;
        
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(TriggerNavigationTutorial);
        CharacterManager.Instance.PlayerController.OnStartedInvestigation.AddListener(TriggerInvestigationTutorial);
        CharacterManager.Instance.PlayerController.OnClueFound.AddListener(TriggerCluesTutorial);
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

    #region Properties

    public UnityEvent OnTutorialTriggered
    {
        get { return onTutorialTriggered; }
    }

    #endregion
}