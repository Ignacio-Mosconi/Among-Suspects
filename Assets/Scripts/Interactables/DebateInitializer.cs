using System;
using UnityEngine;

[System.Serializable]
public struct DebateCharacterSprite
{
    public CharacterName characterName;
    public SpriteRenderer spriteRenderer;
}

public class DebateInitializer : Interactable
{
    [SerializeField] DebateInfo debateInfo;
    [SerializeField] GameObject debateSpritesContainer;
    [SerializeField] DebateCharacterSprite[] debateCharactersSprites;

    protected override void Interact()
    {
        DisableInteraction();
        playerController.SetAvailability(enable: false);
        GameManager.Instance.SetCursorAvailability(enable: true);
        ChapterManager.Instance.ShowDebateStartPrompt();
    }

    public void StartDebate()
    {
        playerController.SetCameraAvailability(enable: false);
        GameManager.Instance.SetCursorAvailability(enable: false);

        debateSpritesContainer.SetActive(true);
        DebateManager.Instance.InitializeDebate(debateInfo, playerController.CluesGathered);
    }

    public void CancelDebate()
    {
        EnableInteraction();
        playerController.SetAvailability(enable: true);
        GameManager.Instance.SetCursorAvailability(enable: false);
    }

    public DebateCharacterSprite[] DebateCharactersSprites
    {
        get { return debateCharactersSprites; }
    }
}