using UnityEngine;

public class NonPlayableCharacter : Interactable
{
    [SerializeField] CharacterName characterName;
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] DialogueInfo dialogueInfo;
    [SerializeField] GameObject characterMesh;
    [SerializeField] Transform leftSpeaker;
    [SerializeField] Transform rightSpeaker;

    bool nameRevealed = false;

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

    public void TriggerNiceReaction()
    {
        dialogueInfo.niceWithPlayer = true;
    }

    #region Getters & Setters

    public CharacterName CharacterName
    {
        get { return characterName; }
    }

    public bool NameRevealed
    {
        get { return nameRevealed; }
        set { nameRevealed = value; }
    }

    public Vector3 LeftSpeakerPosition
    {
        get { return leftSpeaker.position; }
    }

    public Vector3 RightSpeakerPosition
    {
        get { return rightSpeaker.position; }
    }

    public DialogueInfo DialogueInfo
    {
        set { dialogueInfo = value; }
    }

    #endregion
}