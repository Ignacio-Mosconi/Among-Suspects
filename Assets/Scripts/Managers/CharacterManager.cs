using System.Collections.Generic;
using UnityEngine;

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

    Dictionary<string, NonPlayableCharacter> characters = new Dictionary<string, NonPlayableCharacter>();

    void Start()
    {
        NonPlayableCharacter[] npcs = FindObjectsOfType<NonPlayableCharacter>();

        foreach (NonPlayableCharacter npc in npcs)
            characters.Add(npc.CharacterName, npc);
    }

    public NonPlayableCharacter GetCharacter(string characterName)
    {
        return characters[characterName];
    }
}