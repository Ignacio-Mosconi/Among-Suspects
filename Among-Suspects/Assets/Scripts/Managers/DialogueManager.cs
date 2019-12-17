using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(SpeechController))]
[RequireComponent(typeof(CharacterSpriteController))]
[RequireComponent(typeof(DialogueOptionsScreen))]
public class DialogueManager : MonoBehaviour
{
    #region Singleton
    
    static DialogueManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static DialogueManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DialogueManager>();
                if (!instance)
                    Debug.LogError("There is no 'DialogueManager' in the scene");
            }

            return instance;
        }
    }

    #endregion

    [Header("Main UI Elements")]
    [SerializeField] TextMeshProUGUI speakerText = default;
    [SerializeField] TextMeshProUGUI speechText = default;
    [Header("UI Prompts")]
    [SerializeField] UIPrompt speechPanelPrompt = default;
    [SerializeField] UIPrompt leftMouseClickPrompt = default;
    [SerializeField] UIPrompt dialogueOptionsPrompt = default;
    [SerializeField] UIPrompt objectPanelPrompt = default;
    [Header("Icons")]
    [SerializeField] Image speakerIcon = default;
    [SerializeField] Sprite[] speakerSprites = default;

    SpeechController speechController;
    CharacterSpriteController characterSpriteController;
    DialogueOptionsScreen dialogueOptionScreen;
    Dictionary<Language, DialogueInfo> currentDialogueInfoByLanguage;
    Dictionary<Language, ThoughtInfo> currentThoughtInfoByLanguage;
    Dictionary<Language, ItemRequiredThoughtInfo> currentItemRequiredThoughtInfoByLanguage;
    Dictionary<Language, TutorialInfo> currentTutorialInfoByLanguage;
    DialogueInfo currentDialogueInfo;
    ThoughtInfo currentThoughtInfo;
    TutorialInfo currentTutorialInfo;
    ItemRequiredThoughtInfo currentItemRequiredThoughtInfo;
    Dialogue[] currentLines;
    NPC mainSpeaker;
    NPC previousSpeaker;
    int targetSpeechCharAmount;
    int lineIndex;

    UnityEvent onDialogueAreaEnable = new UnityEvent();
    UnityEvent onDialogueAreaDisable = new UnityEvent();

    void Start()
    {
        speechController = GetComponent<SpeechController>();
        characterSpriteController= GetComponent<CharacterSpriteController>();
        dialogueOptionScreen = GetComponent<DialogueOptionsScreen>();
        speechPanelPrompt.SetUp();
        leftMouseClickPrompt.SetUp();
        dialogueOptionsPrompt.SetUp();
        objectPanelPrompt.SetUp();
        enabled = false;

        GameManager.Instance.OnLanguageChanged.AddListener(ChangeCurrentDialogueLanguage);
    }

    void Update()
    {
        if (Input.GetButtonDown("Continue"))
        {
            if (speechController.IsSpeaking())
            {
                speechController.StopSpeaking();
                return;
            }
            
            AudioManager.Instance.PostEvent("Dialogo_Click");
            lineIndex++;

            if (leftMouseClickPrompt.gameObject.activeInHierarchy)
                leftMouseClickPrompt.Hide();
            
            if (lineIndex < currentLines.Length)
                SayDialogue(currentLines[lineIndex]);
            else
            {
                lineIndex = 0;

                if (currentDialogueInfo != null)
                {
                    if (currentLines == currentDialogueInfo.introLines)
                        currentDialogueInfo.introRead = true;
                    if (currentLines == currentDialogueInfo.groupDialogue.dialogue)
                        currentDialogueInfo.groupDialogueRead = true;
                    if (currentLines == currentDialogueInfo.interactiveConversation.intro && 
                        !currentDialogueInfo.interactionOptionSelected)
                        ShowDialogueOptions();
                    else
                        DisableDialogueArea();
                }
                else           
                    DisableDialogueArea();
            }
        }
        else
            if (ShouldDisplayLeftMousePrompt())
                leftMouseClickPrompt.Show();
    }

    void EnableDialogueArea()
    {
        speechPanelPrompt.Show();
        enabled = true;
        onDialogueAreaEnable.Invoke();
    }

    void DisableDialogueArea()
    {
        speechPanelPrompt.Hide();
        enabled = false;

        if (dialogueOptionsPrompt.gameObject.activeInHierarchy)
            dialogueOptionsPrompt.Hide();

        if (currentDialogueInfo && currentLines == currentDialogueInfo.groupDialogue.dialogue && 
            currentDialogueInfo.groupDialogue.cancelOtherGroupDialogues)
            CharacterManager.Instance.CancelOtherGroupDialogues();

        currentDialogueInfoByLanguage = null;
        currentThoughtInfoByLanguage = null;
        currentItemRequiredThoughtInfoByLanguage = null;
        currentTutorialInfoByLanguage = null;
        currentDialogueInfo = null;
        currentThoughtInfo = null;
        currentItemRequiredThoughtInfo = null;
        currentTutorialInfo = null;
        currentLines = null;
        mainSpeaker = null;
        previousSpeaker = null;

        if (objectPanelPrompt.gameObject.activeInHierarchy)
            objectPanelPrompt.Hide();
        characterSpriteController.HideImmediately();

        onDialogueAreaDisable.Invoke();
    }

    void ChangeCurrentDialogueLanguage()
    {
        if (currentDialogueInfo)
        {
            currentDialogueInfo = currentDialogueInfoByLanguage[GameManager.Instance.CurrentLanguage];
            currentLines = currentDialogueInfo.DetermineNextDialogueLines();
            if (currentLines == null)
                currentLines = (mainSpeaker.NiceWithPlayer) ? currentDialogueInfo.niceComment : currentDialogueInfo.rudeComment;
        }

        if (currentThoughtInfo)
        {
            currentThoughtInfo = currentThoughtInfoByLanguage[GameManager.Instance.CurrentLanguage];
            currentLines = (ChapterManager.Instance.CurrentPhase == ChapterPhase.Exploration) ? currentThoughtInfo.explorationThought :
                                                                                                currentThoughtInfo.investigationThought;
        }

        if (currentItemRequiredThoughtInfo)
        {
            bool wasInteractionThought = (currentLines == currentItemRequiredThoughtInfo.interactionThought);
            bool wasUseCorrectThought = (currentLines == currentItemRequiredThoughtInfo.useCorrectItemThought);
            bool wasUseIncorrectThought = (currentLines == currentItemRequiredThoughtInfo.useIncorrectItemThought);

            currentItemRequiredThoughtInfo = currentItemRequiredThoughtInfoByLanguage[GameManager.Instance.CurrentLanguage];
            
            if (wasInteractionThought)
                currentLines = currentItemRequiredThoughtInfo.interactionThought;
            else
                currentLines = (wasUseCorrectThought) ? currentItemRequiredThoughtInfo.useCorrectItemThought : 
                                                        currentItemRequiredThoughtInfo.useIncorrectItemThought;
        }

        if (currentTutorialInfo)
        {
            currentTutorialInfo = currentTutorialInfoByLanguage[GameManager.Instance.CurrentLanguage];
            currentLines = currentTutorialInfo.tutorialLines;
        }

        if (dialogueOptionScreen.IsSelectingOption)
            dialogueOptionScreen.ChangeOptionsLanguage();
    }

    bool ShouldDisplayLeftMousePrompt()
    {
        return (!speechController.IsSpeaking() && !dialogueOptionScreen.IsSelectingOption &&
                !leftMouseClickPrompt.gameObject.activeInHierarchy);
    }

    void SayDialogue(Dialogue dialogue)
    {
        PlayerController playerController = CharacterManager.Instance.PlayerController;

        if (dialogue.clueInfo)
            playerController.AddClue(dialogue.clueInfo);

        if (!String.IsNullOrEmpty(dialogue.dialogueSound.eventName))
            AudioManager.Instance.PostEventDelayed(dialogue.dialogueSound.eventName, dialogue.dialogueSound.playDelay);
        
        if (dialogue.speakerName != playerController.GetCharacterName() && dialogue.speakerName != CharacterName.Tutorial)
        {
            NPC currentSpeaker;
            
            if (!previousSpeaker || dialogue.speakerName != previousSpeaker.GetCharacterName())
            {
                currentSpeaker = (NPC)CharacterManager.Instance.GetCharacter(dialogue.speakerName);
                
                if (currentDialogueInfo)
                {
                    if (currentSpeaker == mainSpeaker)
                    {
                        playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.InteractionPosition);
                        if (previousSpeaker)
                        {
                            float slideDuration = playerController.FirstPersonCamera.GetFocusDuration(mainSpeaker.InteractionPosition);

                            if (previousSpeaker.GetCharacterName() == currentDialogueInfo.groupDialogue.leftSpeaker)
                                characterSpriteController.SlideInFromLeft(slideDuration);
                            else
                                characterSpriteController.SlideInFromRight(slideDuration);
                        }
                    }
                    else
                    {
                        if (currentSpeaker.GetCharacterName() == currentDialogueInfo.groupDialogue.leftSpeaker)
                        {
                            float slideDuration = playerController.FirstPersonCamera.GetFocusDuration(mainSpeaker.LeftSpeakerPosition);

                            playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.LeftSpeakerPosition);
                            characterSpriteController.SlideInFromRight(slideDuration);
                        }
                        else
                        {
                            float slideDuration = playerController.FirstPersonCamera.GetFocusDuration(mainSpeaker.RightSpeakerPosition);

                            playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.RightSpeakerPosition);
                            characterSpriteController.SlideInFromLeft(slideDuration);
                        }
                    }          
                }
                else
                    characterSpriteController.ShowImmediately();

                previousSpeaker = currentSpeaker;
            }
            else
                currentSpeaker = previousSpeaker;

            if (dialogue.revealName)
                currentSpeaker.NameRevealed = true;
            if (dialogue.triggerNiceImpression)
                currentSpeaker.NiceWithPlayer = true;

            speakerText.text = (currentSpeaker.NameRevealed) ? dialogue.speakerName.ToString() : "???";

            if (dialogue.speakerEmotion != CharacterEmotion.Listening)
                characterSpriteController.ChangeSprite(currentSpeaker.GetSprite(dialogue.speakerEmotion));
            
            if (speechText.color != GameManager.Instance.NpcSpeakingTextColor)
                speechText.color = GameManager.Instance.NpcSpeakingTextColor;
            if (speakerText.color != GameManager.Instance.NpcSpeakingTextColor)
                speakerText.color = GameManager.Instance.NpcSpeakingTextColor;
            if (speakerIcon.sprite != speakerSprites[0])
                speakerIcon.sprite = speakerSprites[0];
        }
        else
        {
            speakerText.text = dialogue.speakerName.ToString();

            if (!currentDialogueInfo)
                characterSpriteController.HideImmediately();

            if (dialogue.speakerName != CharacterName.Tutorial)
            {
                if (!dialogue.playerThought)
                {
                    if (speechText.color != GameManager.Instance.PlayerSpeakingTextColor)
                        speechText.color = GameManager.Instance.PlayerSpeakingTextColor;
                    if (speakerText.color != GameManager.Instance.PlayerSpeakingTextColor)
                        speakerText.color = GameManager.Instance.PlayerSpeakingTextColor;
                    if (speakerIcon.sprite != speakerSprites[0])
                        speakerIcon.sprite = speakerSprites[0];
                }
                else
                {
                    if (speechText.color != GameManager.Instance.PlayerThinkingTextColor)
                        speechText.color = GameManager.Instance.PlayerThinkingTextColor;
                    if (speakerText.color != GameManager.Instance.PlayerThinkingTextColor)
                        speakerText.color = GameManager.Instance.PlayerThinkingTextColor;
                    if (speakerIcon.sprite != speakerSprites[1])
                        speakerIcon.sprite = speakerSprites[1];
                }       
            } 
            else
            {
                if (speechText.color != GameManager.Instance.TutorialTextColor)
                    speechText.color = GameManager.Instance.TutorialTextColor;
                if (speakerText.color != GameManager.Instance.TutorialTextColor)
                    speakerText.color = GameManager.Instance.TutorialTextColor;
                if (speakerIcon.sprite != speakerSprites[2])
                    speakerIcon.sprite = speakerSprites[2];
            }
        }

        speechController.StartSpeaking(dialogue.speech);
    }

    void ShowDialogueOptions()
    {
        Dictionary<Language, DialogueOption[]> dialogueOptionsByLanguage = new Dictionary<Language, DialogueOption[]>();

        for (int i = 0; i < (int)Language.Count; i++)
        {
            Language language = (Language)i;
            dialogueOptionsByLanguage.Add(language, currentDialogueInfoByLanguage[language].interactiveConversation.playerOptions);
        }

        enabled = false;
        dialogueOptionsPrompt.Show();
        dialogueOptionScreen.ShowOptionsScreen(dialogueOptionsByLanguage);
    }

    public void ResumeInteractiveDialogue(int option)
    {       
        enabled = true;
        dialogueOptionsPrompt.Hide();
             
        currentLines = currentDialogueInfo.interactiveConversation.resultingDialogues[option].branchDialogue;

        currentDialogueInfo.interactionOptionSelected = true;

        SayDialogue(currentLines[0]);
    }

    public void StartDialogue(Dictionary<Language, DialogueInfo> dialogueInfos, NPC npc)
    {
        currentDialogueInfoByLanguage = dialogueInfos; 
        currentDialogueInfo = currentDialogueInfoByLanguage[GameManager.Instance.CurrentLanguage];
        
        mainSpeaker = npc;
        CharacterManager.Instance.PlayerController.FirstPersonCamera.FocusOnPosition(npc.InteractionPosition);

        characterSpriteController.ShowImmediately();
        
        EnableDialogueArea();

        currentLines = currentDialogueInfo.DetermineNextDialogueLines();
        if (currentLines == null)
            currentLines = (npc.NiceWithPlayer) ? currentDialogueInfo.niceComment : currentDialogueInfo.rudeComment;

        SayDialogue(currentLines[0]);
    }

    public void StartDialogue(Dictionary<Language, ThoughtInfo> thoughtInfos, Vector3 objectPosition, 
                                Sprite objectSprite = null, bool enableImage = false)
    {
        currentThoughtInfoByLanguage = thoughtInfos;
        currentThoughtInfo = currentThoughtInfoByLanguage[GameManager.Instance.CurrentLanguage];

        CharacterManager.Instance.PlayerController.FirstPersonCamera.FocusOnPosition(objectPosition);

        if (enableImage)
        {
            objectPanelPrompt.transform.GetChild(0).GetComponent<Image>().sprite = objectSprite;
            objectPanelPrompt.Show();
        }

        EnableDialogueArea();

        currentLines = (ChapterManager.Instance.CurrentPhase == ChapterPhase.Exploration) ? currentThoughtInfo.explorationThought : 
                                                                                            currentThoughtInfo.investigationThought;

        if (currentThoughtInfo.triggerInvestigationPhase)
            ChapterManager.Instance.TriggerInvestigationPhase();

        SayDialogue(currentLines[0]);
    }

    public void StartDialogue(Dictionary<Language, ItemRequiredThoughtInfo> itemRequiredThoughtInfos, Dialogue[] dialogueLines,
                                Vector3 objectPosition, Sprite objectSprite = null, bool enableImage = false)
    {
        currentItemRequiredThoughtInfoByLanguage = itemRequiredThoughtInfos;
        currentItemRequiredThoughtInfo = currentItemRequiredThoughtInfoByLanguage[GameManager.Instance.CurrentLanguage];
        currentLines = dialogueLines;

        CharacterManager.Instance.PlayerController.FirstPersonCamera.FocusOnPosition(objectPosition);

        if (enableImage)
        {
            objectPanelPrompt.transform.GetChild(0).GetComponent<Image>().sprite = objectSprite;
            objectPanelPrompt.Show();
        }

        EnableDialogueArea();

        SayDialogue(currentLines[0]);
    }

    public void StartDialogue(Dictionary<Language, TutorialInfo> tutorialInfos)
    {
        currentTutorialInfoByLanguage = tutorialInfos;
        currentTutorialInfo = currentTutorialInfoByLanguage[GameManager.Instance.CurrentLanguage];
        currentLines = currentTutorialInfo.tutorialLines;

        EnableDialogueArea();
        SayDialogue(currentLines[0]);
    }

    public void PauseUpdate()
    {
        if (currentLines != null)
        {
            speechPanelPrompt.Hide();
            enabled = false;
        }
    }

    public void ResumeUpdate()
    {
        if (currentLines != null)
        {
            speechPanelPrompt.Show();
            enabled = true;
        }
    }

    #region Properties

    public UnityEvent OnDialogueAreaEnable
    {
        get { return onDialogueAreaEnable; }
    }

    public UnityEvent OnDialogueAreaDisable
    {
        get { return onDialogueAreaDisable; }
    }

    public UIPrompt SpeechPanelPrompt
    {
        get { return speechPanelPrompt; }
    }

    #endregion
}