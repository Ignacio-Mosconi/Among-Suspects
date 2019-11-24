using UnityEngine;

[CreateAssetMenu(fileName = "New Debate Puzzle Menu Text Info", menuName = "Debate Puzzle Menu Text Info", order = 12)]
public class DebatePuzzleMenuTextInfo : ScriptableObject
{
    [Header("Debate Puzzle Lost Screen")]
    public string finalArgumentButtonText = default;
}