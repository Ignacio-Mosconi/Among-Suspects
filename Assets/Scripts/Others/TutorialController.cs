using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialController : MonoBehaviour
{   
    [SerializeField] TutorialTrigger debateTutorialTrigger = default;

    Dictionary<Language, TutorialInfo[]> tutorialsByLanguage = new Dictionary<Language, TutorialInfo[]>();
    float navigationTutorialDelay;
    float investigationTutorialDelay;
    float cluesTutorialDelay;
    float itemsTutorialDelay;
    float debateStartTutorialDelay;

    const float AdditionalTutorialDelay = 0.1f;

    UnityEvent onTutorialTriggered = new UnityEvent();

    void Start()
    {
        LoadTutorials();

        HUD hud = FindObjectOfType<HUD>();

        navigationTutorialDelay = DialogueManager.Instance.SpeechPanelPrompt.HideAnimationDuration + AdditionalTutorialDelay;
        investigationTutorialDelay = hud.InvestigationPhasePrompt.GetOnScreenDuration() + AdditionalTutorialDelay;
        cluesTutorialDelay = hud.ClueFoundPrompt.GetOnScreenDuration() + AdditionalTutorialDelay;
        itemsTutorialDelay = DialogueManager.Instance.SpeechPanelPrompt.HideAnimationDuration + AdditionalTutorialDelay;
        debateStartTutorialDelay = cluesTutorialDelay;
        
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(TriggerNavigationTutorial);
        CharacterManager.Instance.PlayerController.OnStartedInvestigation.AddListener(TriggerInvestigationTutorial);
        CharacterManager.Instance.PlayerController.OnClueFound.AddListener(TriggerCluesTutorial);
        CharacterManager.Instance.PlayerController.OnItemCollected.AddListener(TriggerItemsTutorial);
        CharacterManager.Instance.PlayerController.OnAllCluesFound.AddListener(TriggerDebateStartTutorial);

        debateTutorialTrigger.gameObject.SetActive(false);
        debateTutorialTrigger.OnTrigger.AddListener(DisplayDebateTutorial);
    }

    void LoadTutorials()
    {
        for (int i = 0; i < (int)Language.Count; i++)
        {
            Language language = (Language)i;
            string languagePath = Enum.GetName(typeof(Language), language);
            TutorialInfo[] tutorials = Resources.LoadAll<TutorialInfo>("Tutorials/" + languagePath);

            tutorialsByLanguage.Add(language, tutorials);
        }
    }

    Dictionary<Language, TutorialInfo> FetchTutorialByLanguage(TutorialType tutorialType)
    {
        Dictionary<Language, TutorialInfo> tutorialByLanguage = new Dictionary<Language, TutorialInfo>();

        for (int i = 0; i < (int)Language.Count; i++)
        {
            Language language = (Language)i;
            TutorialInfo tutorial = Array.Find(tutorialsByLanguage[language], t => t.tutorialType == tutorialType);

            tutorialByLanguage.Add(language, tutorial);
        }

        if (tutorialByLanguage == null)
            Debug.LogError("There are no '" + tutorialType.ToString() + "' tutorials set up");

        return tutorialByLanguage;
    }

    void DisplayTutorial(Dictionary<Language, TutorialInfo> tutorialByLanguage)
    {
        DialogueManager.Instance.StartDialogue(tutorialByLanguage);
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

    void TriggerItemsTutorial()
    {
        Invoke("DisplayItemsTutorial", itemsTutorialDelay);
    }

    void TriggerDebateStartTutorial()
    {
        debateTutorialTrigger.gameObject.SetActive(true);
        Invoke("DisplayDebateStartTutorial", debateStartTutorialDelay);
    }

    void DisplayNavigationTutorial()
    {
        Dictionary<Language, TutorialInfo> tutorialByLanguage = FetchTutorialByLanguage(TutorialType.Navigation);
        DialogueManager.Instance.OnDialogueAreaDisable.RemoveListener(TriggerNavigationTutorial);
        DisplayTutorial(tutorialByLanguage);
    }

    void DisplayInvestigationTutorial()
    {
        Dictionary<Language, TutorialInfo> tutorialByLanguage = FetchTutorialByLanguage(TutorialType.Investigation);
        CharacterManager.Instance.PlayerController.OnStartedInvestigation.RemoveListener(TriggerInvestigationTutorial);
        DisplayTutorial(tutorialByLanguage);
    }

    void DisplayCluesTutorial()
    {
        Dictionary<Language, TutorialInfo> tutorialByLanguage = FetchTutorialByLanguage(TutorialType.Clues);
        CharacterManager.Instance.PlayerController.OnClueFound.RemoveListener(TriggerCluesTutorial);
        DisplayTutorial(tutorialByLanguage);
    }

    void DisplayItemsTutorial()
    {
        Dictionary<Language, TutorialInfo> tutorialByLanguage = FetchTutorialByLanguage(TutorialType.Items);
        CharacterManager.Instance.PlayerController.OnItemCollected.RemoveListener(TriggerItemsTutorial);
        DisplayTutorial(tutorialByLanguage);
    }

    void DisplayDebateStartTutorial()
    {
        Dictionary<Language, TutorialInfo> tutorialByLanguage = FetchTutorialByLanguage(TutorialType.DebateStart);
        CharacterManager.Instance.PlayerController.OnAllCluesFound.RemoveListener(TriggerDebateStartTutorial);
        DisplayTutorial(tutorialByLanguage);
    }

    void DisplayDebateTutorial()
    {
        Dictionary<Language, TutorialInfo> tutorialByLanguage = FetchTutorialByLanguage(TutorialType.Debate);
        debateTutorialTrigger.OnTrigger.RemoveListener(DisplayDebateTutorial);
        DisplayTutorial(tutorialByLanguage);
    }

    #region Properties

    public UnityEvent OnTutorialTriggered
    {
        get { return onTutorialTriggered; }
    }

    #endregion
}