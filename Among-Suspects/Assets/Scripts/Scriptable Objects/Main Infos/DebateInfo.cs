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
    public CharacterEmotion speakerEmotion;
    public DialogueSound dialogueSound;
}

[System.Serializable]
public struct Argument
{
    [Header("Argument Intro")]
    public Dialogue[] argumentIntroDialogue;
    [Header("Debate")]
    public DebateDialogue[] debateDialogue;
    [Header("Correct Options")]
    public DebateReaction correctReaction;
    public ClueInfo correctEvidence;
    [Header("Post-Selection Dialogue")]
    public Dialogue[] trustDialogue;
    public Dialogue[] refuteCorrectDialogue;
    public Dialogue[] refuteIncorrectDialogue;
    public Dialogue[] outOfTimeDialogue;
}

[CreateAssetMenu(fileName = "New Debate Info", menuName = "Debate Info", order = 3)]
public class DebateInfo : ScriptableObject
{
    public Argument[] arguments;
    public Dialogue[] loseDebateDialogue;
    public Dialogue[] winDebateDialogue;
    public Dialogue[] finalArgumentDebateDialogue;
}