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
                                currentLines[lineIndex].characterEmotion);
                else
                {
                    lineIndex = 0;
                    if (currentLines == currentDialogueInfo.introLines)
                        currentDialogueInfo.introRead = true;
                    if (currentDialogueInfo.interactiveConversation[0].playerOption != null)
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

    void SayDialogue(string speech, string speakerName = "", CharacterEmotion speakerEmotion = CharacterEmotion.Listening)
    {
        speechText.maxVisibleCharacters = 0;
        speechText.text = speech;
        speakerText.text = speakerName;
        targetSpeechCharAmount = speech.Length;
        
        if (speakerName != "Player")
        {
            NonPlayableCharacter speaker = CharacterManager.Instance.GetCharacter(speakerName);

            if (speakerEmotion != CharacterEmotion.Listening)
                speakerImage.sprite = speaker.GetSprite(speakerEmotion);
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        optionsPanel.SetActive(true);

        for (int i = 0; i < currentDialogueInfo.interactiveConversation.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(true);
            
            TextMeshProUGUI optionText = optionsButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();
            optionText.text = currentDialogueInfo.interactiveConversation[i].playerOption;
        }
    }

    public void SelectDialogueOption(int option)
    {
        for (int i = 0; i < currentDialogueInfo.interactiveConversation.Length; i++)
            optionsButtons[i].gameObject.SetActive(false);

        currentLines = currentDialogueInfo.interactiveConversation[option].dialogue;

        SayDialogue(currentLines[0].speech, currentLines[0].speakerName, currentLines[0].characterEmotion);
        
        enabled = true;
    }

    public void EnableDialogueArea(DialogueInfo dialogueInfo, Vector3 characterPosition)
    {
        currentDialogueInfo = dialogueInfo;
        
        player.firstPersonCamera.FocusOnObject(characterPosition);
        
        SetDialogueAreaAvailability(enableDialogueArea: true);

        if (!dialogueInfo.introRead)
        {
            currentLines = currentDialogueInfo.introLines;
            SayDialogue(currentLines[0].speech, currentLines[0].speakerName, currentLines[0].characterEmotion);
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