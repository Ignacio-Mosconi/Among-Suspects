﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(SpeechController))]
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

    [SerializeField] GameObject dialogueArea = default;
    [SerializeField] GameObject textArea = default;
    [SerializeField] TextMeshProUGUI speakerText = default;
    [SerializeField] TextMeshProUGUI speechText = default;
    [SerializeField] Image speakerImage = default;
    [SerializeField] Image objectImage = default;
    
    SpeechController speechController;
    DialogueOptionsScreen dialogueOptionScreen;
    DialogueInfo currentDialogueInfo;
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
        dialogueOptionScreen = GetComponent<DialogueOptionsScreen>();
        enabled = false;
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
            
            lineIndex++;
            
            if (lineIndex < currentLines.Length)
                SayDialogue(currentLines[lineIndex].speech, 
                            currentLines[lineIndex].speakerName,
                            currentLines[lineIndex].characterEmotion,
                            currentLines[lineIndex].revealName,
                            currentLines[lineIndex].playerThought,
                            currentLines[lineIndex].clueInfo);
            else
            {
                lineIndex = 0;

                if (currentDialogueInfo != null)
                {
                    if (currentLines == currentDialogueInfo.introLines)
                        currentDialogueInfo.introRead = true;
                    if (currentLines == currentDialogueInfo.groupDialogue.dialogue)
                        currentDialogueInfo.groupDialogueRead = true;

                    if (currentDialogueInfo.HasInteractiveDialogue() && !currentDialogueInfo.interactionOptionSelected)
                        ShowDialogueOptions();
                    else
                        SetDialogueAreaAvailability(enableDialogueArea: false);             
                }
                else
                    SetDialogueAreaAvailability(enableDialogueArea: false);              
            }
        }
    }

    void SetDialogueAreaAvailability(bool enableDialogueArea)
    {
        if (enableDialogueArea)
            onDialogueAreaEnable.Invoke();
        else
            onDialogueAreaDisable.Invoke();

        CharacterManager.Instance.PlayerController.SetAvailability(enable: !enableDialogueArea);

        if (!enableDialogueArea)
        {
            if (currentDialogueInfo && currentLines == currentDialogueInfo.groupDialogue.dialogue && 
                currentDialogueInfo.groupDialogue.cancelOtherGroupDialogues)
                CharacterManager.Instance.CancelOtherGroupDialogues();

            currentDialogueInfo = null;
            currentLines = null;
            mainSpeaker = null;
            previousSpeaker = null;

            // if (foundClueInDialogue)
            // {
            //     foundClueInDialogue = false;
            //     onClueFound.Invoke();
            // }

            // if (triggeredInvestigationInDialogue)
            // {
            //     triggeredInvestigationInDialogue = false;
            //     ChapterManager.Instance.TriggerInvestigationPhase();
            // }

            objectImage.gameObject.SetActive(false);
            speakerImage.gameObject.SetActive(false);
        }
        
        dialogueArea.SetActive(enableDialogueArea);
        enabled = enableDialogueArea;
    }

    void SayDialogue(string speech, CharacterName speakerName, CharacterEmotion speakerEmotion, 
                        bool revealName, bool playerThought, ClueInfo clueInfo)
    {
        PlayerController playerController = CharacterManager.Instance.PlayerController;

        if (clueInfo)
            playerController.AddClue(clueInfo);
        
        if (speakerName != playerController.GetCharacterName())
        {
            NPC currentSpeaker;
            
            if (!previousSpeaker || speakerName != previousSpeaker.GetCharacterName())
            {
                currentSpeaker = (NPC)CharacterManager.Instance.GetCharacter(speakerName);
                
                if (currentDialogueInfo)
                {
                    if (currentSpeaker == mainSpeaker)
                        playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.InteractionPosition);
                    else
                    {
                        if (currentSpeaker.GetCharacterName() == currentDialogueInfo.groupDialogue.leftSpeaker)
                            playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.LeftSpeakerPosition);
                        else
                            playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.RightSpeakerPosition);
                    }          
                }
                else
                    speakerImage.gameObject.SetActive(true);

                previousSpeaker = currentSpeaker;
            }
            else
                currentSpeaker = previousSpeaker;

            if (revealName)
                currentSpeaker.NameRevealed = true;

            speakerText.text = (currentSpeaker.NameRevealed) ? speakerName.ToString() : "???";

            if (speakerEmotion != CharacterEmotion.Listening)
                speakerImage.sprite = currentSpeaker.GetSprite(speakerEmotion);
            
            if (speechText.color != GameManager.Instance.NpcSpeakingTextColor)
                speechText.color = GameManager.Instance.NpcSpeakingTextColor;
        }
        else
        {
            speakerText.text = speakerName.ToString();

            if (!currentDialogueInfo)
                speakerImage.gameObject.SetActive(false);
            
            if (!playerThought)
            {
                if (speechText.color != GameManager.Instance.PlayerSpeakingTextColor)
                    speechText.color = GameManager.Instance.PlayerSpeakingTextColor;
            }
            else
            {
                if (speechText.color != GameManager.Instance.PlayerThinkingTextColor)
                    speechText.color = GameManager.Instance.PlayerThinkingTextColor;
            }       
        }

        speechController.StartSpeaking(speech);
    }

    void ShowDialogueOptions()
    {
        enabled = false;
        dialogueOptionScreen.ShowOptionsScreen(currentDialogueInfo.interactiveConversation);
    }

    public void ResumeInteractiveDialogue(int option)
    {       
        enabled = true;
             
        currentLines = currentDialogueInfo.interactiveConversation[option].dialogue;
        mainSpeaker.NiceWithPlayer = currentDialogueInfo.interactiveConversation[option].triggerNiceImpression;

        currentDialogueInfo.interactionOptionSelected = true;

        SayDialogue(currentLines[0].speech, 
                    currentLines[0].speakerName, 
                    currentLines[0].characterEmotion,
                    currentLines[0].revealName,
                    currentLines[0].playerThought,
                    currentLines[0].clueInfo);
    }

    public void EnableDialogueArea(DialogueInfo dialogueInfo, NPC npc)
    {
        currentDialogueInfo = dialogueInfo;
        
        mainSpeaker = npc;
        CharacterManager.Instance.PlayerController.FirstPersonCamera.FocusOnPosition(npc.InteractionPosition);

        speakerImage.gameObject.SetActive(true);
        
        SetDialogueAreaAvailability(enableDialogueArea: true);

        if (currentDialogueInfo.HasIntroLines() && !currentDialogueInfo.introRead)
            currentLines = currentDialogueInfo.introLines;
        else
        {
            if (currentDialogueInfo.HasGroupDialogue() && !currentDialogueInfo.groupDialogueRead)
                currentLines = currentDialogueInfo.groupDialogue.dialogue;
            else
                currentLines = (npc.NiceWithPlayer) ? currentDialogueInfo.niceComment : 
                                                        currentDialogueInfo.rudeComment;
        }

        SayDialogue(currentLines[0].speech,
                    currentLines[0].speakerName,
                    currentLines[0].characterEmotion,
                    currentLines[0].revealName,
                    currentLines[0].playerThought,
                    currentLines[0].clueInfo);
    }

    public void EnableDialogueArea(ThoughtInfo thoughtInfo, Vector3 objectPosition, Sprite objectSprite = null, bool enableImage = false)
    {
        CharacterManager.Instance.PlayerController.FirstPersonCamera.FocusOnPosition(objectPosition);

        if (enableImage)
        {
            objectImage.sprite = objectSprite;
            objectImage.gameObject.SetActive(true);
        }

        SetDialogueAreaAvailability(enableDialogueArea: true);

        currentLines = (ChapterManager.Instance.CurrentPhase == ChapterPhase.Exploration) ? thoughtInfo.explorationThought : 
                                                                                            thoughtInfo.investigationThought;

        if (thoughtInfo.triggerInvestigationPhase)
            ChapterManager.Instance.TriggerInvestigationPhase();

        SayDialogue(currentLines[0].speech,
                    currentLines[0].speakerName,
                    currentLines[0].characterEmotion,
                    currentLines[0].revealName,
                    currentLines[0].playerThought,
                    currentLines[0].clueInfo);
    }

    public void SetUpdateEnable(bool enable)
    {
        if (currentLines != null)
        {
            textArea.SetActive(enable);
            enabled = enable;
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

    #endregion
}