using UnityEngine;

[CreateAssetMenu(fileName = "New Clue Info", menuName = "Clue Info", order = 2)]
public class ClueInfo : ScriptableObject
{
    public string clueName;
    public uint clueID;
    [TextArea(3, 10)] public string description;
    public Sprite clueSprite;
}