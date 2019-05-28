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

    List<NonPlayableCharacter> characters = new List<NonPlayableCharacter>();
    PlayerController playerController = default;

    string dialoguesPath;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        dialoguesPath = "Dialogues/" + SceneManager.GetActiveScene().name + "/";

        NonPlayableCharacter[] npcs = FindObjectsOfType<NonPlayableCharacter>();

        foreach (NonPlayableCharacter npc in npcs)
        {
            if (!characters.Find(c => c.CharacterName == npc.CharacterName))
            {
                if (npc.CharacterName == CharacterName.Monica)
                {
                    npc.NameRevealed = true;
                    npc.NiceWithPlayer = true;
                }
                characters.Add(npc);
            }
            else
                Debug.LogError("There are duplicate characters in the scene.", npc.gameObject);
        }
    }

    public NonPlayableCharacter GetCharacter(CharacterName characterName)
    {
        NonPlayableCharacter character = characters.Find(c => c.CharacterName == characterName);
        
        if (!character)
            Debug.LogError("There are no characters named '" + characterName + "' in the scene.", gameObject);
        
        return character;
    }

    public void LoadInvestigationDialogues()
    {
        foreach (NonPlayableCharacter npc in characters)
        {
            npc.DialogueInfo = Resources.Load(dialoguesPath + npc.CharacterName.ToString() + " Investigation Phase") as DialogueInfo;
            npc.DialogueInfo.introRead = false;
            npc.DialogueInfo.interactionOptionSelected = false;
            npc.DialogueInfo.groupDialogueRead = false;
        }
    }

    public void CancelOtherGroupDialogues()
    {
        foreach (NonPlayableCharacter npc in characters)
        {
            npc.DialogueInfo.groupDialogueRead = true;
        }
    }

    public PlayerController PlayerController
    {
        get { return playerController; }
    }
}