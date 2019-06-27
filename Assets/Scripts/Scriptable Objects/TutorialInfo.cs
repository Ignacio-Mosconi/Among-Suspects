using UnityEngine;

public enum TutorialType
{
    Navigation,
    Investigation,
    Clues,
    DebateStart,
    Debate
}

[CreateAssetMenu(fileName = "New Tutorial Info", menuName = "Tutorial Info", order = 5)]
public class TutorialInfo : ScriptableObject
{
    public TutorialType tutorialType;
    public Dialogue[] tutorialLines;
}