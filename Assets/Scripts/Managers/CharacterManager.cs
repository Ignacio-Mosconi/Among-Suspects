using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CharacterName
{
    James, Monica, William, Gary,
    None
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

    string dialoguesPath;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        characters.Add(playerController);

        dialoguesPath = "Dialogues/" + SceneManager.GetActiveScene().name + "/";

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
                string path = dialoguesPath + character.GetCharacterName().ToString();
                
                path += (chapterPhase == ChapterPhase.Exploration) ? " Exploration Phase" : " Investigation Phase";

                npc.DialogueInfo = Resources.Load(path) as DialogueInfo;
                npc.DialogueInfo.introRead = false;
                npc.DialogueInfo.interactionOptionSelected = false;
                npc.DialogueInfo.groupDialogueRead = false;
            }
        }
    }

    public void CancelOtherGroupDialogues()
    {
        foreach (ICharacter character in characters)
        {
            NPC npc = character as NPC;
            
            if (npc)
                npc.DialogueInfo.groupDialogueRead = true;
        }
    }

    #region Properties

    public PlayerController PlayerController
    {
        get { return playerController; }
    }

    #endregion
}