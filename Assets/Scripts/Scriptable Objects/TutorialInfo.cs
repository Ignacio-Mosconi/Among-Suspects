using UnityEngine;

public enum TutorialType
{
    Navigation,
    Investigation,
    ClueChecking,
    DebateInitialization
}

[CreateAssetMenu(fileName = "New Tutorial Info", menuName = "Tutorial Info", order = 5)]
public class TutorialInfo : ScriptableObject
{
    public TutorialType tutorialType;
    public Dialogue[] tutorialLines;
}