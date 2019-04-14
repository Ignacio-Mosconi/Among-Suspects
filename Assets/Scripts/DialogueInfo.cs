using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 1)]
public class DialogueInfo : ScriptableObject
{
    public string speakerName;
    [TextArea(3, 10)] public string[] speech;
}