using UnityEngine;

public class NonPlayableCharacter : Interactable
{
    [SerializeField] string characterName;
    [SerializeField] Sprite[] characterSprites;
    [SerializeField] DialogueInfo dialogueInfo;

    protected override void Interact()
    {
        DialogueManager.Instance.EnableDialogueArea(dialogueInfo);
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