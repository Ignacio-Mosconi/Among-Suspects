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

    Camera debateCamera;

    void Awake()
    {
        debateCamera = GetComponentInChildren<Camera>(includeInactive: true);
    }

    protected override void Interact()
    {
        DisableInteraction();
        playerController.SetAvailability(enable: false);
        GameManager.Instance.SetCursorAvailability(enable: true);
    }

    public void StartDebate()
    {
        playerController.SetCameraAvailability(enable: false);

        debateSpritesContainer.SetActive(true);
        DebateManager.Instance.EnableDebateArea(this, playerController.CluesGathered);
    }

    public void CancelDebate()
    {
        EnableInteraction();
        playerController.SetAvailability(enable: true);
        GameManager.Instance.SetCursorAvailability(enable: false);
    }

    public SpriteRenderer GetCharacterSpriteRenderer(CharacterName characterName)
    {
        DebateCharacterSprite charSprite = Array.Find(debateCharactersSprites, cs => cs.characterName == characterName);

        return charSprite.spriteRenderer;
    } 

    public Camera DebateCamera
    {
        get { return debateCamera; }
    }

    public DebateInfo DebateInfo
    {
        get { return debateInfo; }
    }
}