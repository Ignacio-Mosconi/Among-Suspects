using UnityEngine;

[CreateAssetMenu(fileName = "New Clue Options Menu Text Info", menuName = "Clue Options Menu Text Info", order = 12)]
public class ClueOptionsMenuTextInfo : ScriptableObject
{
    [Header("Clues Screen")]
    public string selectEvidenceTitle = default;
    public string backButtonText = default;
    public string useButtonText = default;
}