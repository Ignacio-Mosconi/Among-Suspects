using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public enum DebatePhase
{
    Dialoguing, Arguing, SolvingArgument, SolvingCase
}

[RequireComponent(typeof(SpeechController))]
[RequireComponent(typeof(ArgumentController))]
[RequireComponent(typeof(DebateCameraController))]
[RequireComponent(typeof(CredibilityBarController))]
public class DebateManager : MonoBehaviour
{
    #region Singleton

    static DebateManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static DebateManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DebateManager>();
                if (!instance)
                    Debug.LogError("There is no 'Debate Manager' in the scene");
            }

            return instance;
        }
    }

    #endregion

    [Header("Scoring Management")]
    [SerializeField] DebatePerformanceController debatePerformanceController;
    [Header("Main Area")]
    [SerializeField] GameObject debateArea = default;
    [Header("Panels & Sub Areas")]
    [SerializeField] UIPrompt speakerArea = default;
    [SerializeField] UIPrompt argumentAndSpeechArea = default;
    [SerializeField] GameObject argumentPanel = default;
    [SerializeField] GameObject speechPanel = default;
    [SerializeField] UIPrompt debateOptionsPanel = default;
    [SerializeField] AnimatedMenuScreen clueOptionsPanel = default;
    [Header("Buttons & Texts")]
    [SerializeField] Button useEvidenceButton = default;
    [SerializeField] TextMeshProUGUI speakerText = default;
    [SerializeField] TextMeshProUGUI argumentText = default;
    [SerializeField] TextMeshProUGUI speechText = default;
    [Header("Other Properties")]
    [SerializeField] UIPrompt leftClickPrompt = default;
    [SerializeField] [Range(3f, 10f)] float speakerAreaAutoHideTime = 5f;

    SpeechController speechController;
    ArgumentController argumentController;
    DebateCameraController debateCameraController;
    CredibilityBarController credibilityBarController;
    ArgumentTimer argumentTimer;
    CluesScreen cluesScreen;
    DebateCharacterSprite[] debateCharactersSprites;
    Dictionary<Language, DebateInfo> debateInfosByLanguage;
    DebateInfo currentDebateInfo;
    Argument currentArgument;
    Dialogue[] currentDialogueLines;
    DebateDialogue[] currentArgumentLines;
    CharacterName previousSpeaker = CharacterName.None;
    ClueInfo currentlySelectedEvidence;
    DebatePhase currentPhase = DebatePhase.Dialoguing;
    UnityAction playSoundOnFocusFinishAction;
    bool caseWon = false;
    int lineIndex = 0;
    int argumentIndex = 0;
    bool isSelectingOption;

    void Start()
    {
        DebateInitializer debateInitializer = FindObjectOfType<DebateInitializer>();

        speechController = GetComponent<SpeechController>();
        argumentController = GetComponent<ArgumentController>();
        debateCameraController = GetComponent<DebateCameraController>();
        credibilityBarController = GetComponent<CredibilityBarController>();
        argumentTimer = GetComponent<ArgumentTimer>();

        debatePerformanceController = new DebatePerformanceController();

        cluesScreen = GetComponentInChildren<CluesScreen>(includeInactive: true);

        useEvidenceButton.interactable = false;

        GameManager.Instance.AddCursorPointerHoverEventsToAllButtons(debateOptionsPanel.gameObject);
        GameManager.Instance.AddCursorPointerHoverEventsToAllButtons(clueOptionsPanel.gameObject);

        speakerArea.SetUp();
        argumentAndSpeechArea.SetUp();
        debateOptionsPanel.SetUp();
        clueOptionsPanel.SetUp();
        leftClickPrompt.SetUp();

        argumentController.OnArgumentFinish.AddListener(ShowDebateOptions);
        debateCameraController.OnFocusFinish.AddListener(ProceedAfterCameraFocus);
        argumentTimer.OnTimeOut.AddListener(StayQuietAfterComment);
        
        debateCharactersSprites = debateInitializer.DebateCharactersSprites;
        
        Camera debateCamera = debateInitializer.GetComponentInChildren<Camera>(includeInactive: true);
        debateCameraController.SetUpDebateCamera(debateCamera);

        enabled = false;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.D) && CanContinueDialogue())
            SkipDebate();
#endif
        if (Input.GetButtonDown("Continue") && CanContinueDialogue())
        {
            if (debateCameraController.IsFocusing())
            {
                debateCameraController.StopFocusing();
                return;
            }

            if (argumentController.IsExpanding())
            {
                argumentController.StopExpanding(lineIndex == currentArgumentLines.Length - 1);
                return;
            }

            if (speechController.IsSpeaking())
            {
                speechController.StopSpeaking();
                return;
            }

            AudioManager.Instance.PlaySound("Advance Dialogue");

            lineIndex++;

            switch (currentPhase)
            {
                case DebatePhase.Dialoguing:
                    
                    if (lineIndex < currentDialogueLines.Length)
                        Dialogue(currentDialogueLines[lineIndex]);
                    else
                        StartArgumentPhase();
                    break;

                case DebatePhase.Arguing:
                    
                    if (lineIndex < currentArgumentLines.Length)
                        Argue(currentArgumentLines[lineIndex]);
                    break;

                case DebatePhase.SolvingArgument:

                    if (lineIndex < currentDialogueLines.Length)
                        Dialogue(currentDialogueLines[lineIndex]);
                    else
                    {
                        if (!debatePerformanceController.ShouldLoseCase(argumentIndex))
                        {
                            if (argumentIndex == currentDebateInfo.arguments.Length - 1)
                                EndCase();
                            else
                                StartNextArgument();
                        }
                        else
                            EndCase(lose: true);
                    }
                    break;

                case DebatePhase.SolvingCase:
                    
                    if (lineIndex < currentDialogueLines.Length)
                        Dialogue(currentDialogueLines[lineIndex]);
                    else
                    {
                        ChapterManager.Instance.ShowDebateEndScreen(caseWon);              
                        SetDebateAreaAvailability(enableDebateArea: false);
                    }

                    break;
            }

            if (leftClickPrompt.gameObject.activeInHierarchy)
            {
                if (!argumentAndSpeechArea.IsHiding)
                    leftClickPrompt.Hide();
                else
                    leftClickPrompt.Deactivate();
            }
        }
        else
            if (ShouldDisplayLeftClickPrompt())
                leftClickPrompt.Show();
    }

    bool CanContinueDialogue()
    {
        return (!speakerArea.IsShowing && !argumentAndSpeechArea.IsShowing && !speakerArea.IsHiding && !argumentAndSpeechArea.IsHiding);
    }

    bool ShouldDisplayLeftClickPrompt()
    {
        return (!speechController.IsSpeaking() && !argumentController.IsExpanding() && !debateCameraController.IsFocusing() &&
                !isSelectingOption && !leftClickPrompt.gameObject.activeInHierarchy);
    }

    void SetDebateAreaAvailability(bool enableDebateArea)
    {
        debateArea.SetActive(enableDebateArea);
        enabled = enableDebateArea;

        credibilityBarController.ResetCredibilityBar(debatePerformanceController.InitialCredibility);

        if (!enableDebateArea)
        {
            currentDebateInfo = null;
            currentDialogueLines = null;
            currentArgumentLines = null;

            currentPhase = DebatePhase.Dialoguing;

            caseWon = false;
            lineIndex = 0;
            argumentIndex = 0;

            argumentController.ResetArgumentPanelScale();
            ResetMainUIVisibility();
        }
    }

    void ResetMainUIVisibility()
    {
        if (speakerArea.gameObject.activeInHierarchy)
        {
            if (IsInvoking("AutoHideSpeakerArea"))
                CancelInvoke("AutoHideSpeakerArea");
            speakerArea.Hide();
        }
        if (argumentAndSpeechArea.gameObject.activeInHierarchy)
            argumentAndSpeechArea.Hide();
    }

    void ShowSpeakerArea()
    {
        speakerArea.Show();
        Invoke("AutoHideSpeakerArea", speakerAreaAutoHideTime);
    }

    void AutoHideSpeakerArea()
    {
        speakerArea.Hide();
    }

    void DetermineSpeechTextColor(Dialogue dialogue)
    {
        if (dialogue.playerThought)
        {
            if (speakerText.color != GameManager.Instance.PlayerThinkingTextColor)
                speechText.color = GameManager.Instance.PlayerThinkingTextColor;
        }
        else
        {
            if (speechText.color != GameManager.Instance.NpcSpeakingTextColor)
                speechText.color = GameManager.Instance.NpcSpeakingTextColor;
        }
    }

    void ChangeSpeakerNameText(string speakerName)
    {
        speakerText.text = speakerName;
    }

    void ChangeArgumentText(string argument)
    {
        argumentText.text = argument;
        argumentController.ResetArgumentPanelScale();
    }

    void ScheduleSoundPlaybackOnFocusStart(DialogueSound dialogueSound)
    {
        if (dialogueSound.audioClip)
        {
            AudioClip audioClip = dialogueSound.audioClip;
            float playDelay = dialogueSound.playDelay;
            playSoundOnFocusFinishAction = () => PlayDialogueSoundOnFocusFinish(audioClip, playDelay);

            debateCameraController.OnFocusFinish.AddListener(playSoundOnFocusFinishAction);
        }
    }

    void PlayDialogueSoundOnFocusFinish(AudioClip audioClip, float delay)
    {
        AudioManager.Instance.PlaySoundDelayed(audioClip, delay);
        debateCameraController.OnFocusFinish.RemoveListener(playSoundOnFocusFinishAction);
        playSoundOnFocusFinishAction = null;
    }

    void ShowDebateOptions()
    {
        enabled = false;
        isSelectingOption = true;
        debateOptionsPanel.Show();
        argumentTimer.StartTimer();
        GameManager.Instance.SetCursorEnable(enable: true);
    }

    void StartArgumentPhase()
    {
        lineIndex = 0;
        speechPanel.SetActive(false);
        currentPhase = DebatePhase.Arguing;
        ChapterManager.Instance.SetPauseAvailability(enable: false);
        Argue(currentArgumentLines[0]);
    }

    void ProceedAfterCameraFocus()
    {
        if (currentPhase == DebatePhase.Arguing)
            SayArgument(lineIndex == currentArgumentLines.Length - 1, newSpeaker: true);
        else
            SayDialogue(currentDialogueLines[lineIndex].speech, newSpeaker: true);
    }

    void ProceedAfterOptionSelection(bool increaseCredibility)
    {
        currentPhase = DebatePhase.SolvingArgument;

        ChapterManager.Instance.SetPauseAvailability(enable: true);

        argumentController.ResetArgumentPanelScale();
        
        if (currentDialogueLines[0].speakerName != previousSpeaker)
            ResetMainUIVisibility();

        argumentTimer.StopTimer();
        
        float timeLeft = argumentTimer.LastRemainingTimeOnStop;
        float totalAnsweringTime = argumentTimer.LastAvailableAnsweringTime;

        if (increaseCredibility)
            debatePerformanceController.IncreaseCredibility(timeLeft, totalAnsweringTime);
        else
            debatePerformanceController.DecreaseCredibility(timeLeft, totalAnsweringTime);

        argumentPanel.SetActive(false);
        GameManager.Instance.SetCursorEnable(false);

        if (credibilityBarController.IsFillingBar())
            credibilityBarController.StopFillingBar();

        float credibility = debatePerformanceController.Credibility;
        float requiredCredibility = debatePerformanceController.RequiredCredibility;
        bool isAtCriticalCredibility = debatePerformanceController.IsAtCriticalCredibility(argumentIndex);

        credibilityBarController.StartFillingBar(credibility);

        lineIndex = 0;
        isSelectingOption = false;
        enabled = true;

        Dialogue(currentDialogueLines[0]);
    }

    void StartNextArgument()
    {
        lineIndex = 0;
        argumentIndex++;

        currentArgument = currentDebateInfo.arguments[argumentIndex];
        currentArgumentLines = currentArgument.debateDialogue;
        currentDialogueLines = currentArgument.argumentIntroDialogue;
        
        currentPhase = DebatePhase.Dialoguing;

        Dialogue(currentDialogueLines[0]);
    }

    void EndCase(bool lose = false)
    {
        currentPhase = DebatePhase.SolvingCase;
        caseWon = !lose;
        lineIndex = 0;
        currentDialogueLines = (!lose) ? currentDebateInfo.winDebateDialogue : currentDebateInfo.loseDebateDialogue;

        Dialogue(currentDialogueLines[0]);
    }

    void Argue(DebateDialogue debateDialogue)
    {
        DebateCharacterSprite debateSprite = Array.Find(debateCharactersSprites, cs => cs.characterName == debateDialogue.speakerName);
        SpriteRenderer characterRenderer = debateSprite.spriteRenderer;

        ICharacter character = CharacterManager.Instance.GetCharacter(debateDialogue.speakerName);
        characterRenderer.sprite = character.GetSprite(debateDialogue.speakerEmotion);

        if (debateDialogue.speakerName != previousSpeaker)
        {
            Vector3 charPosition = characterRenderer.transform.position;

            ResetMainUIVisibility();

            float changeTextDelay = Mathf.Max(speakerArea.HideAnimationDuration, argumentAndSpeechArea.HideAnimationDuration);

            GameManager.Instance.InvokeMethodInScaledTime(ChangeSpeakerNameText, debateDialogue.speakerName.ToString(), changeTextDelay);
            GameManager.Instance.InvokeMethodInScaledTime(ChangeArgumentText, debateDialogue.argument, changeTextDelay);

            ScheduleSoundPlaybackOnFocusStart(debateDialogue.dialogueSound);

            debateCameraController.StartFocusing(charPosition);     
            previousSpeaker = debateDialogue.speakerName;
        }
        else
        {
            if (debateDialogue.dialogueSound.audioClip)
                AudioManager.Instance.PlaySoundDelayed(debateDialogue.dialogueSound.audioClip, debateDialogue.dialogueSound.playDelay);
            ChangeArgumentText(debateDialogue.argument);
            SayArgument(lineIndex == currentArgumentLines.Length - 1);
        }
    }

    void Dialogue(Dialogue dialogue)
    {
        DebateCharacterSprite debateSprite = Array.Find(debateCharactersSprites, cs => cs.characterName == dialogue.speakerName);
        SpriteRenderer characterRenderer = Array.Find(debateCharactersSprites, cs => cs.characterName == dialogue.speakerName).spriteRenderer;

        ICharacter character = CharacterManager.Instance.GetCharacter(dialogue.speakerName);
        characterRenderer.sprite = character.GetSprite(dialogue.speakerEmotion);

        if (dialogue.speakerName != previousSpeaker)
        {
            Vector3 charPosition = characterRenderer.transform.position;

            ResetMainUIVisibility();

            float changeTextDelay = Mathf.Max(speakerArea.HideAnimationDuration, argumentAndSpeechArea.HideAnimationDuration);
            
            GameManager.Instance.InvokeMethodInScaledTime(DetermineSpeechTextColor, dialogue, changeTextDelay);
            GameManager.Instance.InvokeMethodInScaledTime(ChangeSpeakerNameText, dialogue.speakerName.ToString(), changeTextDelay);

            ScheduleSoundPlaybackOnFocusStart(dialogue.dialogueSound);

            debateCameraController.StartFocusing(charPosition);     
            previousSpeaker = dialogue.speakerName;
        }
        else
        {
            if (dialogue.dialogueSound.audioClip)
                AudioManager.Instance.PlaySoundDelayed(dialogue.dialogueSound.audioClip, dialogue.dialogueSound.playDelay);
            DetermineSpeechTextColor(dialogue);
            SayDialogue(dialogue.speech);
        }
    }

    void SayArgument(bool lastArgument, bool newSpeaker = false)
    {
        if (newSpeaker)
            ShowSpeakerArea();
        argumentAndSpeechArea.Show();
        argumentPanel.SetActive(true);

        argumentController.StartExpanding(lastArgument);
    }

    void SayDialogue(string speech, bool newSpeaker = false)
    {
        if (newSpeaker)
            ShowSpeakerArea();
        argumentAndSpeechArea.Show();
        speechPanel.SetActive(true);

        speechController.StartSpeaking(speech);
    }

    void LoadRefuteOptionsCallbacks()
    {
        Button[] cluesButtons = cluesScreen.CluesButtons.ToArray();
        
        for (int i = 0; i < cluesButtons.Length; i++)
        {
            Button clueButton = cluesButtons[i];
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);

            clueButton.onClick.AddListener(() => ChangeCurrentlySelectedEvidence(clueInfo));
        }
    }

    void ChangeCurrentlySelectedEvidence(ClueInfo clueInfo)
    {
        currentlySelectedEvidence = clueInfo;
        useEvidenceButton.interactable = true;
    }

    void StayQuietAfterComment()
    {
        currentDialogueLines = currentArgument.outOfTimeDialogue;
        
        if (debateOptionsPanel.gameObject.activeInHierarchy)
            debateOptionsPanel.Hide();
        if (clueOptionsPanel.gameObject.activeInHierarchy)
            clueOptionsPanel.Hide();

        ProceedAfterOptionSelection(increaseCredibility: false);
    }

    public void TrustComment()
    {
        bool increaseCredibility = (currentArgument.correctReaction == DebateReaction.Agree);

        currentDialogueLines = currentArgument.trustDialogue;  
        debateOptionsPanel.Hide();
        ProceedAfterOptionSelection(increaseCredibility);
    }

    public void RefuteComment()
    {
        AudioManager.Instance.PlaySound("Button Click");

        ResetMainUIVisibility();
        debateOptionsPanel.Hide();
        clueOptionsPanel.Show();

        if (CharacterManager.Instance.PlayerController.CluesGathered.Count > 0)
        {
            for (int i = 0; i < ChapterManager.Instance.CluesAmount; i++)
            {
                ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);
                
                if (CharacterManager.Instance.PlayerController.HasClue(ref clueInfo))
                {
                    ChangeCurrentlySelectedEvidence(clueInfo);
                    break;
                }
            }
        }
        
        LoadRefuteOptionsCallbacks();
    }

    public void AccuseWithEvidence()
    {  
        bool increaseCredibility;

        if (currentArgument.correctReaction == DebateReaction.Disagree && 
            currentArgument.correctEvidence.clueID == currentlySelectedEvidence.clueID)
        {
            currentDialogueLines = currentArgument.refuteCorrectDialogue;
            increaseCredibility = true;
        }
        else
        {
            currentDialogueLines = currentArgument.refuteIncorrectDialogue;
            increaseCredibility = false;
        }

        clueOptionsPanel.Hide();
        
        useEvidenceButton.interactable = false;
        currentlySelectedEvidence = null;

        ProceedAfterOptionSelection(increaseCredibility);
    }

    public void ReturnToDebateOptions()
    {
        AudioManager.Instance.PlaySound("Button Click");

        clueOptionsPanel.Hide();
        debateOptionsPanel.Show();
        ShowSpeakerArea();
        argumentAndSpeechArea.Show();
    }

    public void StartDebate(Dictionary<Language, DebateInfo> debateInfos, List<ClueInfo> playerClues)
    {
        debateCameraController.SetDebateCameraAvailability(enable: true);

        debateInfosByLanguage = debateInfos;
        currentDebateInfo = debateInfosByLanguage[GameManager.Instance.CurrentLanguage];
        currentArgument = currentDebateInfo.arguments[0];
        currentDialogueLines = currentArgument.argumentIntroDialogue;
        currentArgumentLines = currentArgument.debateDialogue;

        debatePerformanceController.Initialize(currentDebateInfo.arguments.Length);
        SetDebateAreaAvailability(enableDebateArea: true);

        GameManager.Instance.OnLanguageChanged.AddListener(ChangeDebateLanguage);

        ChangeSpeakerNameText(currentDialogueLines[0].speakerName.ToString());
        Dialogue(currentDialogueLines[0]);
    }

    public void ChangeDebateLanguage()
    {
        bool agreedInLastArgument = false;
        bool selectedCorrectReactionInLastArgument = false;

        if (currentPhase == DebatePhase.SolvingArgument)
        {
            agreedInLastArgument = (currentDialogueLines == currentArgument.trustDialogue);
            selectedCorrectReactionInLastArgument = (currentArgument.correctReaction == DebateReaction.Agree && 
                                                        currentDialogueLines == currentArgument.trustDialogue) ||
                                                        (currentDialogueLines == currentArgument.refuteCorrectDialogue);
        }

        currentDebateInfo = debateInfosByLanguage[GameManager.Instance.CurrentLanguage];
        currentArgument = currentDebateInfo.arguments[argumentIndex];
        
        switch (currentPhase)
        {
            case DebatePhase.Dialoguing:
                currentDialogueLines = currentArgument.argumentIntroDialogue;
                break;

            case DebatePhase.Arguing:
                currentArgumentLines = currentArgument.debateDialogue;
                break;

            case DebatePhase.SolvingArgument:
                if (agreedInLastArgument)
                    currentDialogueLines = currentArgument.trustDialogue;
                else
                    currentDialogueLines = (selectedCorrectReactionInLastArgument) ? currentArgument.refuteCorrectDialogue :
                                                                                    currentArgument.refuteIncorrectDialogue;
                break;

            case DebatePhase.SolvingCase:
                currentDialogueLines = (caseWon) ? currentDebateInfo.winDebateDialogue : currentDebateInfo.loseDebateDialogue;
                break;
        }
    }

    public void SetUpdateEnable(bool enable)
    {
        if (currentDebateInfo)
        {
            debateArea.SetActive(enable);
            if (!isSelectingOption)
                enabled = enable;
        }
    }

    #region Properties

    public DebatePerformanceController DebatePerformanceController
    {
        get { return debatePerformanceController; }
    }

    #endregion

#if UNITY_EDITOR
    #region Development Cheats

    void SkipDebate()
    {
        for (int i = argumentIndex; i < currentDebateInfo.arguments.Length; i++)
            debatePerformanceController.IncreaseCredibility(60f, 60f);

        lineIndex = 0;
        caseWon = true;
        currentDialogueLines = currentDebateInfo.winDebateDialogue;
        currentPhase = DebatePhase.SolvingCase;
        Dialogue(currentDialogueLines[0]);
    }

    #endregion
#endif
}