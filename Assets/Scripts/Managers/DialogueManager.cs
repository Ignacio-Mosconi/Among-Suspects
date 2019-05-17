using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

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

    // [SerializeField] Color playerSpeakingTextColor;
    // [SerializeField] Color playerThinkingTextColor;
    // [SerializeField] Color npcSpeakingTextColor;
    [SerializeField] GameObject dialogueArea;
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] TextMeshProUGUI speechText;
    [SerializeField] Image speakerImage;
    [SerializeField] Image objectImage;
    [SerializeField] VerticalLayoutGroup optionsLayout;

    PlayerController playerController;
    DialogueInfo currentDialogueInfo;
    Dialogue[] currentLines;
    Coroutine speakingRoutine;
    Button[] optionsButtons;
    NonPlayableCharacter mainSpeaker;
    NonPlayableCharacter previousSpeaker;
    float characterShowIntervals;
    float textSpeedMultiplier;
    int targetSpeechCharAmount;
    int lineIndex;
    int[] regularOptionsLayoutPadding = { 0, 0 };
    float regularOptionsLayoutSpacing = 0f;

    UnityEvent onDialogueAreaEnable = new UnityEvent();
    UnityEvent onDialogueAreaDisable = new UnityEvent();

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        characterShowIntervals = 1f / GameManager.Instance.TargetFrameRate;
        textSpeedMultiplier = 1f / GameManager.Instance.TextSpeedMultiplier;

        optionsButtons = optionsLayout.GetComponentsInChildren<Button>(includeInactive: true);
        
        regularOptionsLayoutPadding[0] = optionsLayout.padding.top;
        regularOptionsLayoutPadding[1] = optionsLayout.padding.bottom;
        regularOptionsLayoutSpacing = optionsLayout.spacing;

        enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Continue"))
        {
            if (speakingRoutine != null)
                StopSpeaking();
            else
            {
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
                            ShowOptionsMenu();
                        else
                            SetDialogueAreaAvailability(enableDialogueArea: false);             
                    }
                    else
                        SetDialogueAreaAvailability(enableDialogueArea: false);              
                }
            }
        }
    }

    void SetDialogueAreaAvailability(bool enableDialogueArea)
    {
        if (enableDialogueArea)
            onDialogueAreaEnable.Invoke();
        else
            onDialogueAreaDisable.Invoke();

        playerController.SetAvailability(enable: !enableDialogueArea);

        if (!enableDialogueArea)
        {
            currentDialogueInfo = null;
            currentLines = null;

            objectImage.gameObject.SetActive(false);
            speakerImage.gameObject.SetActive(false);
        }
        
        dialogueArea.SetActive(enableDialogueArea);
        enabled = enableDialogueArea;
    }

    void SetDialogueAreaAvailabilityForDebate(bool enableForDebate)
    {
        dialogueArea.SetActive(enableForDebate);
    }

    void SayDialogue(string speech, CharacterName speakerName, CharacterEmotion speakerEmotion, 
                        bool revealName, bool playerThought, ClueInfo clueInfo)
    {
        speechText.maxVisibleCharacters = 0;
        speechText.text = speech;
        targetSpeechCharAmount = speech.Length;

        if (clueInfo)
            playerController.AddClue(clueInfo);
        
        if (speakerName != playerController.PlayerName)
        {
            NonPlayableCharacter currentSpeaker;
            
            if (!previousSpeaker || speakerName != previousSpeaker.CharacterName)
            {
                currentSpeaker = CharacterManager.Instance.GetCharacter(speakerName);
                
                if (currentSpeaker == mainSpeaker)
                    playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.InteractionPosition);
                else
                {
                    if (currentSpeaker.CharacterName == currentDialogueInfo.groupDialogue.leftSpeaker)
                        playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.LeftSpeakerPosition);
                    else
                        playerController.FirstPersonCamera.FocusOnPosition(mainSpeaker.RightSpeakerPosition);
                }
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

            previousSpeaker = currentSpeaker;
        }
        else
        {
            speakerText.text = speakerName.ToString();
            
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

        speakingRoutine = StartCoroutine(Speak());
    }

    void StopSpeaking()
    {
        if (speakingRoutine != null)
            StopCoroutine(speakingRoutine);
        speechText.maxVisibleCharacters = targetSpeechCharAmount;
        speakingRoutine = null;
    }

    IEnumerator Speak()
    {
        while (speechText.maxVisibleCharacters != targetSpeechCharAmount)
        {
            speechText.maxVisibleCharacters++;          
            yield return new WaitForSecondsRealtime(characterShowIntervals * textSpeedMultiplier);
        }

        speakingRoutine = null;
    }

    void ShowOptionsMenu()
    {
        enabled = false;

        GameManager.Instance.SetCursorAvailability(enable: true);

        int i = 0;

        for (i = 0; i < currentDialogueInfo.interactiveConversation.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(true);
            
            TextMeshProUGUI[] optionTexts = optionsButtons[i].gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            optionTexts[0].text = currentDialogueInfo.interactiveConversation[i].playerOption.option;
            optionTexts[1].text = currentDialogueInfo.interactiveConversation[i].playerOption.description;
        }

        int optionsLayoutPaddingMult = optionsButtons.Length - i;
        int addtionalPadding = (int)optionsButtons[0].GetComponent<Image>().rectTransform.sizeDelta.y / optionsButtons.Length;

        optionsLayout.padding.top = regularOptionsLayoutPadding[0] + (addtionalPadding * optionsLayoutPaddingMult);
        optionsLayout.padding.bottom = regularOptionsLayoutPadding[1] + (addtionalPadding * optionsLayoutPaddingMult);
        optionsLayout.spacing = regularOptionsLayoutSpacing + (addtionalPadding * optionsLayoutPaddingMult);
    }

    public void SelectDialogueOption(int option)
    {       
        GameManager.Instance.SetCursorAvailability(enable: false);
        enabled = true;
        
        for (int i = 0; i < currentDialogueInfo.interactiveConversation.Length; i++)
            optionsButtons[i].gameObject.SetActive(false);

        optionsLayout.padding.top = regularOptionsLayoutPadding[0];
        optionsLayout.padding.bottom = regularOptionsLayoutPadding[1];
        optionsLayout.spacing = regularOptionsLayoutSpacing;
             
        currentLines = currentDialogueInfo.interactiveConversation[option].dialogue;
        currentDialogueInfo.niceWithPlayer = currentDialogueInfo.interactiveConversation[option].triggerNiceImpression;
        currentDialogueInfo.interactionOptionSelected = true;

        SayDialogue(currentLines[0].speech, 
                    currentLines[0].speakerName, 
                    currentLines[0].characterEmotion,
                    currentLines[0].revealName,
                    currentLines[0].playerThought,
                    currentLines[0].clueInfo);
    }

    public void EnableDialogueArea(DialogueInfo dialogueInfo, NonPlayableCharacter npc)
    {
        currentDialogueInfo = dialogueInfo;
        
        mainSpeaker = npc;
        playerController.FirstPersonCamera.FocusOnPosition(npc.InteractionPosition);

        speakerImage.gameObject.SetActive(true);
        
        SetDialogueAreaAvailability(enableDialogueArea: true);

        if (currentDialogueInfo.HasIntroLines() && !currentDialogueInfo.introRead)
            currentLines = currentDialogueInfo.introLines;
        else
        {
            if (currentDialogueInfo.HasGroupDialogue() && !currentDialogueInfo.groupDialogueRead)
                currentLines = currentDialogueInfo.groupDialogue.dialogue;
            else
                currentLines = (currentDialogueInfo.niceWithPlayer) ? currentDialogueInfo.niceComment : 
                                                                        currentDialogueInfo.rudeComment;
        }

        SayDialogue(currentLines[0].speech,
                    currentLines[0].speakerName,
                    currentLines[0].characterEmotion,
                    currentLines[0].revealName,
                    currentLines[0].playerThought,
                    currentLines[0].clueInfo);
    }

    public void EnableDialogueArea(Dialogue[] thoughts, Vector3 objectPosition, Sprite objectSprite = null, bool enableImage = false)
    {
        playerController.FirstPersonCamera.FocusOnPosition(objectPosition);

        if (enableImage)
        {
            objectImage.sprite = objectSprite;
            objectImage.gameObject.SetActive(true);
        }

        SetDialogueAreaAvailability(enableDialogueArea: true);

        currentLines = thoughts;

        SayDialogue(currentLines[0].speech,
                    currentLines[0].speakerName,
                    currentLines[0].characterEmotion,
                    currentLines[0].revealName,
                    currentLines[0].playerThought,
                    currentLines[0].clueInfo);
    }

    #region Getters & Setters

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