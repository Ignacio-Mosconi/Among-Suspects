using UnityEngine;

[CreateAssetMenu(fileName = "New Debate Puzzle Menu Text Info", menuName = "Debate Puzzle Menu Text Info", order = 12)]
public class DebatePuzzleMenuTextInfo : ScriptableObject
{
    [Header("Intro Texts")]
    public string solveTheCaseTitleText = default;
    public string[] instructionTexts = default;


    [Header("Debate Puzzle Finished")]
    public string finalArgumentButtonText = default;
}