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

    public override void Interact()
    {
        DisableInteraction();
        playerController.Disable();
        GameManager.Instance.SetCursorEnable(enable: true);
        ChapterManager.Instance.ShowDebateStartConfirmation();
    }

    public void StartDebate()
    {
        playerController.DeactivateCamera();
        GameManager.Instance.SetCursorEnable(enable: false);

        debateSpritesContainer.SetActive(true);
        CharacterManager.Instance.HideCharacterMeshes();
        DebateManager.Instance.StartDebate(debateInfo, playerController.CluesGathered);
    }

    public void CancelDebate()
    {
        EnableInteraction();
        playerController.Enable();
        GameManager.Instance.SetCursorEnable(enable: false);
    }

    public DebateCharacterSprite[] DebateCharactersSprites
    {
        get { return debateCharactersSprites; }
    }
}