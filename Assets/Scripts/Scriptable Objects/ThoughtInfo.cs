using UnityEngine;

[CreateAssetMenu(fileName = "New Thought Info", menuName = "Thought Info", order = 4)]
public class ThoughtInfo : ScriptableObject
{
    public Dialogue[] explorationThought;
    public Dialogue[] investigationThought;
    public bool triggerInvestigationPhase;
}