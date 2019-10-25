using UnityEngine;

[CreateAssetMenu(fileName = "New Thought Info", menuName = "Thought Info", order = 4)]
public class ThoughtInfo : ScriptableObject
{
    [Header("Dialogues")]
    public Dialogue[] explorationThought;
    public Dialogue[] investigationThought;
    [Header("Other Properties")]
    public bool triggerInvestigationPhase;
}