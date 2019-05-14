using UnityEngine;

public enum DebateReaction
{
    Trust, Doubt, Disagree, Accuse
}

public struct DebateDialogue
{
    public CharacterName speakerName;
    [TextArea(3, 10)] public string speech;
    public CharacterEmotion characterEmotion;
    public bool playerThought;
}

public struct Argument
{
    DebateDialogue[] debateDialogue;
    DebateReaction correctReaction;
    ClueInfo correctEvidence;
}

[CreateAssetMenu(fileName = "New Debate Info", menuName = "Debate Info", order = 3)]
public class DebateInfo : ScriptableObject
{
    Argument[] arguments;
}