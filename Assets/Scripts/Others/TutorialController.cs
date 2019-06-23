using System;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] [Range(0.5f, 1f)] float tutorialDisplayDelay = 0.75f;
    
    TutorialInfo[] tutorials;

    void Start()
    {
        tutorials = Resources.LoadAll<TutorialInfo>("Tutorials");
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(TriggerNavigationTutorial);
    }

    void TriggerNavigationTutorial()
    {
        Invoke("DisplayNavigationTutorial", tutorialDisplayDelay);
    }

    void DisplayNavigationTutorial()
    {
        TutorialInfo tutorial = Array.Find(tutorials, t => t.tutorialType == TutorialType.Navigation);

        if (!tutorial)
        {
            Debug.LogError("There are no 'Navigation' tutorials set up");
            return;
        }

        DialogueManager.Instance.OnDialogueAreaDisable.RemoveListener(TriggerNavigationTutorial);
        DialogueManager.Instance.StartDialogue(tutorial);
    }
}