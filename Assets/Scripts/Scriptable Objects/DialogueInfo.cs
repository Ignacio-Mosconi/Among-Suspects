using UnityEngine;

public enum CharacterEmotion
{
    Normal, Alternative, Happy, Surprised,
    Angry, Mad, Accusing, Shocked, 
    Listening
}

[System.Serializable]
public struct Dialogue
{
    public string speakerName;
    [TextArea(3, 10)] public string speech;
    public CharacterEmotion characterEmotion;
}

[System.Serializable]
public struct InteractiveDialogue
{
    public string playerOption;
    public Dialogue[] dialogue;
}

[CreateAssetMenu(fileName = "New Dialogue Info", menuName = "Dialogue Info", order = 1)]
public class DialogueInfo : ScriptableObject
{
    [Header("Intro Dialogue")]
    public Dialogue[] introLines;
    [Header("Interactive Dialogue")]
    public InteractiveDialogue[] interactiveConversation;

    [HideInInspector] public bool introRead = false;
}