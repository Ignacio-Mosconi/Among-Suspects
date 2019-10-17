using UnityEngine;

[CreateAssetMenu(fileName = "New Debate Options Menu Text Info", menuName = "Debate Options Menu Text Info", order = 13)]
public class DebateOptionsMenuTextInfo : ScriptableObject
{
    [Header("Clues Screen")]
    public string trust = default;
    public string refute = default;
}