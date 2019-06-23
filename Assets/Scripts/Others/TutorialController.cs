using System;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] [Range(0.5f, 2.5f)] float navigationTutorialDelay = 0.75f;
    [SerializeField] [Range(0.5f, 2.5f)] float investigationTutorialDelay = 0.75f;
    
    TutorialInfo[] tutorials;

    void Start()
    {
        tutorials = Resources.LoadAll<TutorialInfo>("Tutorials");
        
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(TriggerNavigationTutorial);
        CharacterManager.Instance.PlayerController.OnStartedInvestigation.AddListener(TriggerInvestigationTutorial);
    }

    TutorialInfo FetchTutorial(TutorialType tutorialType)
    {
        TutorialInfo tutorial = Array.Find(tutorials, t => t.tutorialType == tutorialType);
        if (!tutorial)
            Debug.LogError("There are no '" + tutorialType.ToString() + "' tutorials set up");

        return tutorial;
    }

    void TriggerNavigationTutorial()
    {
        Invoke("DisplayNavigationTutorial", navigationTutorialDelay);
    }

    void TriggerInvestigationTutorial()
    {
        Invoke("DisplayInvestigationTutorial", investigationTutorialDelay);
    }

    void DisplayNavigationTutorial()
    {
        TutorialInfo tutorial = FetchTutorial(TutorialType.Navigation);
        DialogueManager.Instance.OnDialogueAreaDisable.RemoveListener(TriggerNavigationTutorial);
        DialogueManager.Instance.StartDialogue(tutorial);
    }

    void DisplayInvestigationTutorial()
    {
        TutorialInfo tutorial = FetchTutorial(TutorialType.Investigation);
        CharacterManager.Instance.PlayerController.OnStartedInvestigation.RemoveListener(TriggerInvestigationTutorial);
        DialogueManager.Instance.StartDialogue(tutorial);
    }
}