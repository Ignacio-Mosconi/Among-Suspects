using UnityEngine;

public enum CharacterEmotion
{
    Normal, Alternative, Happy, Surprised,
    Angry, Mad, Accusing, Shocked
}

[System.Serializable]
public struct Dialogue
{
    public string speakerName;
    [TextArea(3, 10)] public string speech;
    public CharacterEmotion characterEmotion;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 1)]
public class DialogueInfo : ScriptableObject
{
    public Dialogue[] lines;
}