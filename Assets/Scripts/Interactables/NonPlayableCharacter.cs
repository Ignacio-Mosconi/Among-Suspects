using UnityEngine;

public class NonPlayableCharacter : Interactable
{
    [SerializeField] string characterName;
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] DialogueInfo dialogueInfo;
    [SerializeField] GameObject characterMesh;

    void Awake()
    {
        dialogueInfo.introRead = false;
        dialogueInfo.interactionOptionSelected = false;
    }

    protected override void EnableInteraction()
    {
        base.EnableInteraction();
        characterMesh.SetActive(true);
    }

    protected override void DisableInteraction()
    {
        base.DisableInteraction();
        characterMesh.SetActive(false);
    }

    protected override void Interact()
    {   
        DisableInteraction();
        DialogueManager.Instance.EnableDialogueArea(dialogueInfo, interactionPoint.position);
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