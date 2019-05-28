using UnityEngine;

public class NonPlayableCharacter : Interactable
{
    [SerializeField] CharacterName characterName = default;
    [SerializeField] Sprite[] characterSprites = default;
    [SerializeField] DialogueInfo dialogueInfo = default;
    [SerializeField] GameObject characterMesh = default;
    [SerializeField] Transform leftSpeaker = default;
    [SerializeField] Transform rightSpeaker = default;

    bool nameRevealed = false;
    bool niceWithPlayer = false;

    void Awake()
    {
        dialogueInfo.introRead = false;
        dialogueInfo.interactionOptionSelected = false;
        dialogueInfo.groupDialogueRead = false;
    }

    public override void EnableInteraction()
    {
        base.EnableInteraction();
        characterMesh.SetActive(true);
    }

    public override void DisableInteraction()
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

    public CharacterName CharacterName
    {
        get { return characterName; }
    }

    public bool NameRevealed
    {
        get { return nameRevealed; }
        set { nameRevealed = value; }
    }

    public bool NiceWithPlayer
    {
        get { return niceWithPlayer; }
        set { niceWithPlayer = value; }
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
        get { return dialogueInfo; }
        set { dialogueInfo = value; }
    }

    #endregion
}