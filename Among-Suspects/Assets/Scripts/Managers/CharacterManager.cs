using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CharacterName
{
    James, Monica, William, Gary,
    None, Tutorial
}

public class CharacterManager : MonoBehaviour
{
    #region Singleton

    static CharacterManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static CharacterManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<CharacterManager>();
                if (!instance)
                    Debug.LogError("There is no 'CharacterManager' in the scene");
            }

            return instance;
        }
    }

    #endregion

    List<ICharacter> characters = new List<ICharacter>();
    PlayerController playerController = default;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        characters.Add(playerController);

        NPC[] npcs = FindObjectsOfType<NPC>();

        foreach (NPC npc in npcs)
        {
            if (characters.Find(c => c.GetCharacterName() == npc.GetCharacterName()) != null)
            {
                Debug.LogError("There are duplicate characters in the scene.", npc.gameObject);
                return;
            }
            
            characters.Add(npc);
        }

        LoadDialogues(ChapterPhase.Exploration);
    }

    public ICharacter GetCharacter(CharacterName characterName)
    {
        ICharacter character = characters.Find(c => c.GetCharacterName() == characterName);
        
        if (character == null)
            Debug.LogError("There are no characters named '" + characterName + "' in the scene.", gameObject);
        
        return character;
    }

    public void LoadDialogues(ChapterPhase chapterPhase)
    {
        foreach (ICharacter character in characters)
        {
            NPC npc = character as NPC;

            if (npc)
            {
                Dictionary<Language, DialogueInfo> dialogueInfosByLanguage = new Dictionary<Language, DialogueInfo>();
                
                for (int i = 0; i < (int)Language.Count; i++)
                {
                    Language language = (Language)i;
                    string dialoguesPath = "Dialogues/" + Enum.GetName(typeof(Language), language) + "/" + 
                                            SceneManager.GetActiveScene().name + "/";
                    
                    dialoguesPath += character.GetCharacterName().ToString();
                    dialoguesPath += (chapterPhase == ChapterPhase.Exploration) ? " Exploration Phase" : " Investigation Phase";

                    DialogueInfo dialogueInfo = Resources.Load(dialoguesPath) as DialogueInfo;

                    dialogueInfo.introRead = false;
                    dialogueInfo.interactionOptionSelected = false;
                    dialogueInfo.groupDialogueRead = false;

                    dialogueInfosByLanguage.Add(language, dialogueInfo);
                }

                npc.SetDialogues(dialogueInfosByLanguage);
            }
        }
    }

    public void CancelOtherGroupDialogues()
    {
        foreach (ICharacter character in characters)
        {
            NPC npc = character as NPC;      
            if (npc)
                npc.DisableGroupDialogue();
        }
    }

    public void HideCharacterMeshes()
    {
        foreach (ICharacter character in characters)
        {
            NPC npc = character as NPC;
            if (npc)
                npc.HideMesh();
        }
    }

    public void ShowCharacterMeshes()
    {
        foreach (ICharacter character in characters)
        {
            NPC npc = character as NPC;
            if (npc)
                npc.ShowMesh();
        }
    }

    #region Properties

    public PlayerController PlayerController
    {
        get { return playerController; }
    }

    #endregion
}