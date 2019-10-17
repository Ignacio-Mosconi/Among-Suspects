using UnityEngine;

[CreateAssetMenu(fileName = "New Item Required Thought Info", menuName = "Item Required Thought Info", order = 5)]
public class ItemRequiredThoughtInfo : ScriptableObject
{
    [Header("Dialogues")]
    public Dialogue[] interactionThought;
    public Dialogue[] useCorrectItemThought;
    public Dialogue[] useIncorrectItemThought;
}