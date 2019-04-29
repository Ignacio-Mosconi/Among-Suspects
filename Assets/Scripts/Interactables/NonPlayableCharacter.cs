using UnityEngine;

public class NonPlayableCharacter : Interactable
{
    [SerializeField] string characterName;
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] DialogueInfo dialogueInfo;
    [SerializeField] GameObject characterMesh;

    protected override void Start()
    {
        base.Start();
        
        DialogueManager.Instance.OnDialogueAreaDisable.AddListener(EnableInteraction);
    }

    protected override void Interact()
    {   
        DisableInteraction();
        DialogueManager.Instance.EnableDialogueArea(dialogueInfo, interactionPoint.position);
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