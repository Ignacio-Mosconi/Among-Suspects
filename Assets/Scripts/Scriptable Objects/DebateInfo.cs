using UnityEngine;

public enum DebateReaction
{
    Agree, Disagree
}

[System.Serializable]
public struct DebateDialogue
{
    public CharacterName speakerName;
    [TextArea(3, 10)] public string argument;
    public CharacterEmotion characterEmotion;
}

[System.Serializable]
public struct Argument
{
    public Dialogue[] argumentIntroDialogue;
    public DebateDialogue[] debateDialogue;
    public DebateReaction correctReaction;
    public ClueInfo correctEvidence;
    public Dialogue[] trustDialogue;
    public Dialogue[] refuteDialogue;
}

[CreateAssetMenu(fileName = "New Debate Info", menuName = "Debate Info", order = 3)]
public class DebateInfo : ScriptableObject
{
    public Argument[] arguments;
}