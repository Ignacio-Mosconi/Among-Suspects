using System.Collections.Generic;
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

    Dictionary<Language, DialogueInfo> dialogueInfoByLanguage;

    static string[] interactionKindByLanguage = { "talk", "hablar" };

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

    public override string GetInteractionKind()
    {
        return interactionKindByLanguage[(int)GameManager.Instance.CurrentLanguage];
    }

    public override void Interact()
    {   
        DisableInteraction();
        DialogueManager.Instance.StartDialogue(dialogueInfoByLanguage, this);
    }

    public void SetDialogues(Dictionary<Language, DialogueInfo> dialogueInfosByLanguage)
    {
        this.dialogueInfoByLanguage = dialogueInfosByLanguage;
    }

    public void ShowMesh()
    {
        characterMesh.SetActive(true);
    }

    public void HideMesh()
    {
        characterMesh.SetActive(false);
    }

    public void DisableGroupDialogue()
    {
        foreach (DialogueInfo dialogueInfo in dialogueInfoByLanguage.Values)
            dialogueInfo.groupDialogueRead = true;
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

    #endregion
}