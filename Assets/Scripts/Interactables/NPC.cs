using UnityEngine;

public class NPC : Interactable, ICharacter
{
    [Header("Main Data")]
    [SerializeField] CharacterName characterName = default;
    [SerializeField] Sprite[] characterSprites = default;
    [Header("Other Data")]
    [SerializeField] bool nameRevealed = false;
    [SerializeField] bool niceWithPlayer = false;
    [Header("Other References")]
    [SerializeField] GameObject characterMesh = default;
    [SerializeField] Transform leftSpeaker = default;
    [SerializeField] Transform rightSpeaker = default;

    DialogueInfo dialogueInfo;

    public override void EnableInteraction()
    {
        base.EnableInteraction();
        CharacterManager.Instance.ShowCharacterMeshes();
    }

    public override void DisableInteraction()
    {
        base.DisableInteraction();
        CharacterManager.Instance.HideCharacterMeshes();
    }

    protected override void Interact()
    {   
        DisableInteraction();
        DialogueManager.Instance.StartDialogue(dialogueInfo, this);
    }

    public void ShowMesh()
    {
        characterMesh.SetActive(true);
    }

    public void HideMesh()
    {
        characterMesh.SetActive(false);
    }
    
    public CharacterName GetCharacterName()
    {
        return characterName;
    }

    public Sprite GetSprite(CharacterEmotion characterEmotion)
    {
        return characterSprites[(int)characterEmotion];
    }

    #region Properties

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