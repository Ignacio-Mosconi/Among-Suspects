using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("UI Elements")]
    [Header("Areas")]
    [SerializeField] GameObject debateArea = default;
    [SerializeField] GameObject speakerArea = default;
    [SerializeField] GameObject argumentAndSpeechArea = default;
    [Header("Panels")]
    [SerializeField] GameObject argumentPanel = default;
    [SerializeField] GameObject speechPanel = default;
    [SerializeField] GameObject debateOptionsPanel = default;
    [SerializeField] GameObject clueOptionsPanel = default;
    [Header("Buttons & Texts")]
    [SerializeField] Button useEvidenceButton = default;
    [SerializeField] TextMeshProUGUI speakerText = default;
    [SerializeField] TextMeshProUGUI argumentText = default;
    [SerializeField] TextMeshProUGUI speechText = default;
    
    SpeechController speechController;
    ArgumentController argumentController;
    DebateCameraController debateCameraController;
    CredibilityBarController credibilityBarController;
    CluesScreen cluesScreen;
    DebateCharacterSprite[] debateCharactersSprites;
    DebateInfo currentDebateInfo;
    Argument currentArgument;
    Dialogue[] currentDialogueLines;
    DebateDialogue[] currentArgumentLines;
    CharacterName previousSpeaker = CharacterName.None;
    ClueInfo currentlySelectedEvidence;
    DebatePhase currentPhase = DebatePhase.Dialoguing;
    bool caseWon = false;
    int lineIndex = 0;
    int argumentIndex = 0;
    float credibilityPerc;
    float credibilityIncPerc;
    float credibilityDecPerc;
    bool isSelectingOption;

    const float MinCredibilityPercRequired = 70f;
    const float InitialCredibilityPerc = 50f;

    void Start()
    {
        DebateInitializer debateInitializer = FindObjectOfType<DebateInitializer>();

        speechController = GetComponent<SpeechController>();
        argumentController = GetComponent<ArgumentController>();
        debateCameraController = GetComponent<DebateCameraController>();
        credibilityBarController = GetComponent<CredibilityBarController>();
        
        cluesScreen = GetComponentInChildren<CluesScreen>(includeInactive: true);

        argumentController.OnArgumentFinish.AddListener(ShowDebateOptions);
        debateCameraController.OnFocusFinish.AddListener(ProceedAfterCameraFocus);
        
        debateCharactersSprites = debateInitializer.DebateCharactersSprites;
        
        Camera debateCamera = debateInitializer.GetComponentInChildren<Camera>(includeInactive: true);
        debateCameraController.SetUpDebateCamera(debateCamera);

        enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Continue"))
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

            lineIndex++;

            switch (currentPhase)
            {
                case DebatePhase.Dialoguing:
                    
                    if (lineIndex < currentDialogueLines.Length)
                    {
                        Dialogue(currentDialogueLines[lineIndex]);
                    }
                    else
                        StartArgumentPhase();
                    break;

                case DebatePhase.Arguing:
                    
                    argumentController.ResetArgumentPanelScale();
                    if (lineIndex < currentArgumentLines.Length)
                    {
                        Argue(currentArgumentLines[lineIndex]);
                    }
                    break;

                case DebatePhase.SolvingArgument:

                    if (lineIndex < currentDialogueLines.Length)
                    {
                        Dialogue(currentDialogueLines[lineIndex]);
                    }
                    else
                    {
                        if (!ShouldLoseCase())
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
                    {
                        Dialogue(currentDialogueLines[lineIndex]);
                    }
                    else
                    {
                        ChapterManager.Instance.ShowDebateEndScreen(caseWon);              
                        SetDebateAreaAvailability(enableDebateArea: false);
                    }

                    break;
            }
        }
    }

    bool ShouldLoseCase()
    {
        bool shouldLose;
        int argumentsRemaining = currentDebateInfo.arguments.Length - argumentIndex - 1;
        float maxAchievableCredibility = credibilityPerc + argumentsRemaining * credibilityIncPerc;

        shouldLose = (maxAchievableCredibility < MinCredibilityPercRequired);
        
        return shouldLose;
    }

    void SetDebateAreaAvailability(bool enableDebateArea)
    {
        debateArea.SetActive(enableDebateArea);
        enabled = enableDebateArea;

        if (!enableDebateArea)
        {
            currentDebateInfo = null;
            currentDialogueLines = null;
            currentArgumentLines = null;

            currentPhase = DebatePhase.Dialoguing;
            
            credibilityPerc = InitialCredibilityPerc;

            caseWon = false;
            lineIndex = 0;
            argumentIndex = 0;

            argumentController.ResetArgumentPanelScale();
            credibilityBarController.ResetCredibilityBar(InitialCredibilityPerc);
            ResetMainUIVisibility();
        }
    }

    void ResetMainUIVisibility()
    {
        speakerArea.SetActive(false);
        argumentAndSpeechArea.SetActive(false);
    }

    void ShowDebateOptions()
    {
        enabled = false;
        isSelectingOption = true;
        debateOptionsPanel.SetActive(true);
        GameManager.Instance.SetCursorEnable(enable: true);
    }

    void StartArgumentPhase()
    {
        lineIndex = 0;
        speechPanel.SetActive(false);
        currentPhase = DebatePhase.Arguing;
        Argue(currentArgumentLines[0]);
    }

    void ProceedAfterCameraFocus()
    {
        if (currentPhase == DebatePhase.Arguing)
            SayArgument(lineIndex == currentArgumentLines.Length - 1);
        else
            SayDialogue(currentDialogueLines[lineIndex].speech);
    }

    void ProceedAfterOptionSelection()
    {
        int argumentsRemaining = currentDebateInfo.arguments.Length - 1 - argumentIndex;
        float percAtNextFail = credibilityPerc - credibilityDecPerc;
        bool isCriticalPer = (percAtNextFail + credibilityIncPerc * argumentsRemaining < MinCredibilityPercRequired);

        argumentController.ResetArgumentPanelScale();
        ResetMainUIVisibility();

        argumentPanel.SetActive(false);
        GameManager.Instance.SetCursorEnable(false);

        if (credibilityBarController.IsFillingBar())
            credibilityBarController.StopFillingBar();

        credibilityBarController.StartFillingBar(credibilityPerc, MinCredibilityPercRequired, isCriticalPer);

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

        argumentText.text = debateDialogue.argument;

        ICharacter character = CharacterManager.Instance.GetCharacter(debateDialogue.speakerName);
        characterRenderer.sprite = character.GetSprite(debateDialogue.speakerEmotion);

        if (debateDialogue.speakerName != previousSpeaker)
        {
            Vector3 charPosition = characterRenderer.transform.position;

            ResetMainUIVisibility();

            debateCameraController.StartFocusing(charPosition);     
            speakerText.text = debateDialogue.speakerName.ToString();
            previousSpeaker = debateDialogue.speakerName;
        }
        else
            SayArgument(lineIndex == currentArgumentLines.Length - 1);
    }

    void Dialogue(Dialogue dialogue)
    {
        DebateCharacterSprite debateSprite = Array.Find(debateCharactersSprites, cs => cs.characterName == dialogue.speakerName);
        SpriteRenderer characterRenderer = Array.Find(debateCharactersSprites, cs => cs.characterName == dialogue.speakerName).spriteRenderer;

        ICharacter character = CharacterManager.Instance.GetCharacter(dialogue.speakerName);
        characterRenderer.sprite = character.GetSprite(dialogue.speakerEmotion);

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

        if (dialogue.speakerName != previousSpeaker)
        {
            Vector3 charPosition = characterRenderer.transform.position;

            ResetMainUIVisibility();

            debateCameraController.StartFocusing(charPosition);
            speakerText.text = dialogue.speakerName.ToString();
            previousSpeaker = dialogue.speakerName;
        }
        else
            SayDialogue(dialogue.speech);
    }

    void SayArgument(bool lastArgument)
    {
        speakerArea.SetActive(true);
        argumentAndSpeechArea.SetActive(true);
        argumentPanel.SetActive(true);

        argumentController.StartExpanding(lastArgument);
    }

    void SayDialogue(string speech)
    {
        speakerArea.SetActive(true);
        argumentAndSpeechArea.SetActive(true);
        speechPanel.SetActive(true);

        speechController.StartSpeaking(speech);
    }

    void ChangeCurrentlySelectedEvidence(ClueInfo clueInfo)
    {
        currentlySelectedEvidence = clueInfo;
        useEvidenceButton.interactable = true;
    }

    public void TrustComment()
    {
        currentDialogueLines = currentArgument.trustDialogue;
        
        currentPhase = DebatePhase.SolvingArgument;

        if (currentArgument.correctReaction == DebateReaction.Agree)
            credibilityPerc += credibilityIncPerc;
        else
            credibilityPerc -= credibilityDecPerc;

        debateOptionsPanel.SetActive(false);

        ProceedAfterOptionSelection();
    }

    public void RefuteComment()
    {
        debateOptionsPanel.SetActive(false);
        speakerArea.SetActive(false);
        argumentAndSpeechArea.SetActive(false);
        clueOptionsPanel.gameObject.SetActive(true);

        Button[] cluesButtons = cluesScreen.CluesButtons.ToArray();

        for (int i = 0; i < cluesButtons.Length; i++)
        {
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);
            cluesButtons[i].onClick.AddListener(() => ChangeCurrentlySelectedEvidence(clueInfo));
        }
    }

    public void AccuseWithEvidence()
    {        
        currentPhase = DebatePhase.SolvingArgument;

        if (currentArgument.correctReaction == DebateReaction.Disagree && 
            currentArgument.correctEvidence == currentlySelectedEvidence)
        {
            currentDialogueLines = currentArgument.refuteCorrectDialogue;
            credibilityPerc += credibilityIncPerc;
        }
        else
        {
            currentDialogueLines = currentArgument.refuteIncorrectDialogue;
            credibilityPerc -= credibilityDecPerc;
        }

        clueOptionsPanel.SetActive(false);
        
        useEvidenceButton.interactable = false;
        currentlySelectedEvidence = null;

        ProceedAfterOptionSelection();
    }

    public void ReturnToDebateOptions()
    {
        debateOptionsPanel.SetActive(true);
        speakerArea.SetActive(true);
        argumentAndSpeechArea.SetActive(true);
        clueOptionsPanel.SetActive(false);
    }

    public void InitializeDebate(DebateInfo debateInfo, List<ClueInfo> playerClues)
    {
        debateCameraController.SetDebateCameraAvailability(enable: true);

        currentDebateInfo = debateInfo;
        currentArgument = currentDebateInfo.arguments[0];
        currentDialogueLines = currentArgument.argumentIntroDialogue;
        currentArgumentLines = currentArgument.debateDialogue;

        credibilityPerc = InitialCredibilityPerc;
        credibilityIncPerc = credibilityPerc / currentDebateInfo.arguments.Length;
        credibilityDecPerc = credibilityIncPerc * 2f;

        credibilityBarController.ResetCredibilityBar(credibilityPerc);

        SetDebateAreaAvailability(enableDebateArea: true);

        Dialogue(currentDialogueLines[0]);
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
}