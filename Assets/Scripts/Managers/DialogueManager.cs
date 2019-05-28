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

    [SerializeField] GameObject dialogueArea = default;
    [SerializeField] GameObject textArea = default;
    [SerializeField] TextMeshProUGUI speakerText = default;
    [SerializeField] TextMeshProUGUI speechText = default;
    [SerializeField] Image speakerImage = default;
    [SerializeField] Image objectImage = default;
    [SerializeField] VerticalLayoutGroup optionsLayout = default;
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
            yield return new WaitForSeconds(characterShowIntervals * textSpeedMultiplier);
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
        mainSpeaker.NiceWithPlayer = currentDialogueInfo.interactiveConversation[option].triggerNiceImpression;

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