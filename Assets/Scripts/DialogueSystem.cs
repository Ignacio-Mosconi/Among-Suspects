using UnityEngine;

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

    Player player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        player.firstPersonCamera = playerObject.GetComponent<FirstPersonCamera>();
        player.playerMovement = playerObject.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
            SetDialogueAreaAvailability(false);
    }

    void SetDialogueAreaAvailability(bool enableDialogueArea)
    {
        HUD.Instance.SetVisibility(!enableDialogueArea);
        player.playerMovement.enabled = !enableDialogueArea;
        player.firstPersonCamera.enabled = !enableDialogueArea;
        
        dialogueArea.SetActive(enableDialogueArea);
        this.enabled = enableDialogueArea;
    }

    public void EnableDialogueArea()
    {
        SetDialogueAreaAvailability(true);
    }
}