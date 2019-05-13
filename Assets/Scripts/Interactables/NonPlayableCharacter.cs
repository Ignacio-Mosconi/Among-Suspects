using UnityEngine;

public class NonPlayableCharacter : Interactable
{
    [SerializeField] string characterName;
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] DialogueInfo dialogueInfo;
    [SerializeField] GameObject characterMesh;
    [SerializeField] Transform leftSpeaker;
    [SerializeField] Transform rightSpeaker;

    void Awake()
    {
        dialogueInfo.introRead = false;
        dialogueInfo.interactionOptionSelected = false;
        dialogueInfo.groupDialogueRead = false;
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
        DialogueManager.Instance.EnableDialogueArea(dialogueInfo, this);
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

    public Vector3 LeftSpeakerPosition
    {
        get { return leftSpeaker.position; }
    }

    public Vector3 RightSpeakerPosition
    {
        get { return rightSpeaker.position; }
    }

    #endregion
}