using System.Collections;
using UnityEngine;
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

    Player player;
    DialogueInfo currentDialogueInfo;
    Coroutine speakingRoutine;
    float characterShowIntervals;
    float textSpeedMultiplier;
    string currentTargetSpeech;
    int currentSpeechIndex;

    UnityEvent onDialogueAreaEnable = new UnityEvent();
    UnityEvent onDialogueAreaDisable = new UnityEvent();

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        player.firstPersonCamera = playerObject.GetComponent<FirstPersonCamera>();
        player.playerMovement = playerObject.GetComponent<PlayerMovement>();

        characterShowIntervals = 1f / GameManager.Instance.TargetFrameRate;
        textSpeedMultiplier = 1f / GameManager.Instance.TextSpeedMultiplier;
    }

    void Update()
    {
        if (Input.GetButtonDown("Continue"))
        {
            if (speakingRoutine != null)
                StopSpeaking();
            else
            {
                currentSpeechIndex++;
                if (currentSpeechIndex < currentDialogueInfo.speech.Length)
                    SayDialogue(currentDialogueInfo.speech[currentSpeechIndex]);
                else
                {
                    currentSpeechIndex = 0;
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
        this.enabled = enableDialogueArea;
    }

    void SayDialogue(string speech)
    {
        speakingRoutine = StartCoroutine(Speak(speech));
    }

    void StopSpeaking()
    {
        StopCoroutine(speakingRoutine);
        speechText.text = currentTargetSpeech;
        speakingRoutine = null;
    }

    IEnumerator Speak(string speech)
    {
        speechText.text = "";
        currentTargetSpeech = speech;

        while (speechText.text != currentTargetSpeech)
        {
            speechText.text += currentTargetSpeech[speechText.text.Length];           
            yield return new WaitForSecondsRealtime(characterShowIntervals * textSpeedMultiplier);
        }

        speakingRoutine = null;
    }

    public void EnableDialogueArea(DialogueInfo dialogueInfo)
    {
        SetDialogueAreaAvailability(true);
        ChangeSpeaker(dialogueInfo);
    }

    public void ChangeSpeaker(DialogueInfo dialogueInfo)
    {
        currentDialogueInfo = dialogueInfo;
        speakerText.text = dialogueInfo.speakerName;
        SayDialogue(dialogueInfo.speech[0]);
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