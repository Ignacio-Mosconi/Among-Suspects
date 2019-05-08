using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

struct Player
{
    public FirstPersonCamera firstPersonCamera;
    public PlayerMovement playerMovement;
}

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

    [SerializeField] string playerName;
    [SerializeField] Color playerSpeakingTextColor;
    [SerializeField] Color npcSpeakingTextColor;
    [SerializeField] GameObject dialogueArea;
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] TextMeshProUGUI speechText;
    [SerializeField] Image speakerImage;
    [SerializeField] GameObject optionsPanel;

    Player player;
    DialogueInfo currentDialogueInfo;
    Dialogue[] currentLines;
    Coroutine speakingRoutine;
    Button[] optionsButtons;
    float characterShowIntervals;
    float textSpeedMultiplier;
    int targetSpeechCharAmount;
    int lineIndex;

    UnityEvent onDialogueAreaEnable = new UnityEvent();
    UnityEvent onDialogueAreaDisable = new UnityEvent();

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        player.firstPersonCamera = playerObject.GetComponent<FirstPersonCamera>();
        player.playerMovement = playerObject.GetComponent<PlayerMovement>();

        characterShowIntervals = 1f / GameManager.Instance.TargetFrameRate;
        textSpeedMultiplier = 1f / GameManager.Instance.TextSpeedMultiplier;

        optionsButtons = optionsPanel.GetComponentsInChildren<Button>(includeInactive: true);

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
                                currentLines[lineIndex].incognito);
                else
                {
                    lineIndex = 0;
                    
                    if (currentLines == currentDialogueInfo.introLines)
                        currentDialogueInfo.introRead = true;
                    
                    if (!currentDialogueInfo.interactionOptionSelected)
                        ShowOptionsMenu();
                    else
                        SetDialogueAreaAvailability(false);
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

        player.playerMovement.enabled = !enableDialogueArea;
        player.firstPersonCamera.enabled = !enableDialogueArea;
        
        dialogueArea.SetActive(enableDialogueArea);
        enabled = enableDialogueArea;
    }

    void SayDialogue(string speech, string speakerName = "", 
                    CharacterEmotion speakerEmotion = CharacterEmotion.Listening, bool incognito = false)
    {
        speechText.maxVisibleCharacters = 0;
        speechText.text = speech;
        speakerText.text = (!incognito) ? speakerName : "???";
        targetSpeechCharAmount = speech.Length;
        
        if (speakerName != playerName)
        {
            NonPlayableCharacter speaker = CharacterManager.Instance.GetCharacter(speakerName);

            if (speakerEmotion != CharacterEmotion.Listening)
                speakerImage.sprite = speaker.GetSprite(speakerEmotion);
            
            if (speechText.color != npcSpeakingTextColor)
                speechText.color = npcSpeakingTextColor;
        }
        else
        {
            if (speechText.color != playerSpeakingTextColor)
                speechText.color = playerSpeakingTextColor;
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

        for (int i = 0; i < currentDialogueInfo.interactiveConversation.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(true);
            
            TextMeshProUGUI[] optionTexts = optionsButtons[i].gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            optionTexts[0].text = currentDialogueInfo.interactiveConversation[i].playerOption.option;
            optionTexts[1].text = currentDialogueInfo.interactiveConversation[i].playerOption.description;
        }

        
    }

    public void SelectDialogueOption(int option)
    {       
        GameManager.Instance.SetCursorAvailability(enable: false);
        enabled = true;
        
        for (int i = 0; i < currentDialogueInfo.interactiveConversation.Length; i++)
            optionsButtons[i].gameObject.SetActive(false);
             
        currentLines = currentDialogueInfo.interactiveConversation[option].dialogue;
        currentDialogueInfo.niceWithPlayer = currentDialogueInfo.interactiveConversation[option].triggerNiceImpression;
        currentDialogueInfo.interactionOptionSelected = true;

        SayDialogue(currentLines[0].speech, 
                    currentLines[0].speakerName, 
                    currentLines[0].characterEmotion,
                    currentLines[0].incognito);
    }

    public void EnableDialogueArea(DialogueInfo dialogueInfo, Vector3 characterPosition)
    {
        currentDialogueInfo = dialogueInfo;
        
        player.firstPersonCamera.FocusOnObject(characterPosition);
        
        SetDialogueAreaAvailability(enableDialogueArea: true);

        if (!dialogueInfo.introRead)
            currentLines = currentDialogueInfo.introLines;
        else
            currentLines = (currentDialogueInfo.niceWithPlayer) ? currentDialogueInfo.niceComment : 
                                                                    currentDialogueInfo.rudeComment;

        SayDialogue(currentLines[0].speech,
            currentLines[0].speakerName,
            currentLines[0].characterEmotion,
            currentLines[0].incognito);
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