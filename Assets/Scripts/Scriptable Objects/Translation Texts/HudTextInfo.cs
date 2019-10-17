using UnityEngine;

[CreateAssetMenu(fileName = "New HUD Text Info", menuName = "HUD Text Info", order = 14)]
public class HudTextInfo : ScriptableObject
{
    [Header("HUD Elements")]
    public string interact = default;
    public string clueFound = default;
    public string[] investigationPhaseInstructions = default;
}