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

    Player player;
    DialogueInfo currentDialogueInfo;
    Coroutine speakingRoutine;
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
                if (lineIndex < currentDialogueInfo.lines.Length)
                    SayDialogue(currentDialogueInfo.lines[lineIndex].speech, 
                                currentDialogueInfo.lines[lineIndex].speakerName,
                                currentDialogueInfo.lines[lineIndex].characterEmotion);
                else
                {
                    lineIndex = 0;
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

    void SayDialogue(string speech, string speakerName = "", CharacterEmotion speakerEmotion = CharacterEmotion.Normal)
    {
        NonPlayableCharacter speaker = CharacterManager.Instance.GetCharacter(speakerName);

        speechText.maxVisibleCharacters = 0;
        speechText.text = speech;
        speakerText.text = speakerName;
        speakerImage.sprite = speaker.GetSprite(speakerEmotion);
        targetSpeechCharAmount = speech.Length;

        speakingRoutine = StartCoroutine(Speak());
    }

    void StopSpeaking()
    {
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

    public void EnableDialogueArea(DialogueInfo dialogueInfo, Vector3 characterPosition)
    {
        currentDialogueInfo = dialogueInfo;
        
        player.firstPersonCamera.FocusOnObject(characterPosition);
        
        SetDialogueAreaAvailability(enableDialogueArea: true);
        SayDialogue(dialogueInfo.lines[0].speech, 
                    dialogueInfo.lines[0].speakerName,
                    dialogueInfo.lines[0].characterEmotion);
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