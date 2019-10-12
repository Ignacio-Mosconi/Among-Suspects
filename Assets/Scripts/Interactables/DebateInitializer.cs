using System;
using System.Collections.Generic;
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
    [Header("Debate Initializer Properties")]
    [SerializeField] GameObject debateSpritesContainer = default;
    [SerializeField] DebateCharacterSprite[] debateCharactersSprites = default;
    
    Dictionary<Language, DebateInfo> debateInfos = new Dictionary<Language, DebateInfo>();

    protected override void Start()
    {
        base.Start();
        
        for (int i = 0; i < (int)Language.Count; i++)
        {
            string languagePath = Enum.GetName(typeof(Language), (Language)i);
            DebateInfo debateInfo = Resources.Load("Debates/" + languagePath + "/" + SceneManager.GetActiveScene().name + " Debate") as DebateInfo;
            
            debateInfos.Add((Language)i, debateInfo);
        }
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
        DebateManager.Instance.StartDebate(debateInfos, playerController.CluesGathered);
    }

    public void CancelDebate()
    {
        EnableInteraction();
        playerController.Enable();
        GameManager.Instance.SetCursorEnable(enable: false);
    }

    public override string GetInteractionKind()
    {
        return "start the debate";
    }

    public DebateCharacterSprite[] DebateCharactersSprites
    {
        get { return debateCharactersSprites; }
    }
}