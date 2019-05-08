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
    public bool incognito;
}

[System.Serializable]
public struct DialogueOption
{
    public string option;
    public string description;
}

[System.Serializable]
public struct InteractiveDialogue
{
    public DialogueOption playerOption;
    public Dialogue[] dialogue;
    public bool triggerNiceImpression;
}

[CreateAssetMenu(fileName = "New Dialogue Info", menuName = "Dialogue Info", order = 1)]
public class DialogueInfo : ScriptableObject
{
    [Header("Intro Dialogue")]
    public Dialogue[] introLines;
    [Header("Interactive Dialogue")]
    public InteractiveDialogue[] interactiveConversation;
    [Header("'Already Interacted' Dialogue")]
    public Dialogue[] niceComment;
    public Dialogue[] rudeComment;

    [HideInInspector] public bool introRead = false;
    [HideInInspector] public bool interactionOptionSelected = false;
    [HideInInspector] public bool niceWithPlayer = false;
}