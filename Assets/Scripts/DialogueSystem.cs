using System.Collections;
using UnityEngine;
using TMPro;

struct Player
{
    public FirstPersonCamera firstPersonCamera;
    public PlayerMovement playerMovement;
}

public class DialogueSystem : MonoBehaviour
{
    #region Singleton
    static DialogueSystem instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static DialogueSystem Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DialogueSystem>();
                if (!instance)
                    Debug.LogError("There is no 'DialogueSystem' in the scene");
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
    string currentTargetSpeech;
    int currentSpeechIndex;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        player.firstPersonCamera = playerObject.GetComponent<FirstPersonCamera>();
        player.playerMovement = playerObject.GetComponent<PlayerMovement>();
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
        HUD.Instance.SetVisibility(!enableDialogueArea);
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

        while (speechText.text != speech)
        {
            speechText.text += speech[speechText.text.Length];           
            yield return new WaitForEndOfFrame();
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
}