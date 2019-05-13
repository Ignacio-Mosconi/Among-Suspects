using System.Collections.Generic;
using UnityEngine;

public enum CharacterName
{
    Player,
    Byakuya,
    Kyoko,
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

    void Start()
    {
        NonPlayableCharacter[] npcs = FindObjectsOfType<NonPlayableCharacter>();

        foreach (NonPlayableCharacter npc in npcs)
        {
            if (!characters.Find(c => c.CharacterName == npc.CharacterName)) 
                characters.Add(npc);
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
}