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
        
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(EnableMeshObject);
    }

    protected override void Interact()
    {
        DisableMeshObject();
        DialogueManager.Instance.EnableDialogueArea(dialogueInfo);
    }

    void EnableMeshObject()
    {
        characterMesh.SetActive(true);
    }

    void DisableMeshObject()
    {
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