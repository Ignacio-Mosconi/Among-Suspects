using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct DebateCharacterSprite
{
    public CharacterName characterName;
    public SpriteRenderer spriteRenderer;
}

public class DebateInitializer : Interactable
{
    [SerializeField] GameObject debateSpritesContainer = default;
    [SerializeField] DebateCharacterSprite[] debateCharactersSprites = default;
    
    DebateInfo debateInfo;

    protected override void Start()
    {
        base.Start();
        debateInfo = Resources.Load("Debates/" + SceneManager.GetActiveScene().name + " Debate") as DebateInfo;
    }

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