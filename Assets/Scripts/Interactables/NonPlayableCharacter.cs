using UnityEngine;

public class NonPlayableCharacter : Interactable
{
    [SerializeField] string characterName;
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] DialogueInfo dialogueInfo;

    GameObject characterMesh;

    void Awake()
    {
        characterMesh = transform.GetChild(0).gameObject;
    }

    protected override void Start()
    {
        base.Start();
        
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(EnableInteraction);
    }

    protected override void Interact()
    {
        DisableInteraction();
        DialogueManager.Instance.EnableDialogueArea(dialogueInfo);
    }

    void EnableInteraction()
    {
        base.enabled = true;
        characterMesh.SetActive(true);
    }

    void DisableInteraction()
    {
        base.enabled = false;
        characterMesh.SetActive(false);
    }
    
    public Sprite GetSprite(CharacterEmotion characterEmotion)
    {
        return characterSprites[(int)characterEmotion];
    }

    #region Getters & Setters

    public string CharacterName
    {
        get { return characterName; }
    }

    #endregion
}