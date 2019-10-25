using UnityEngine;

[CreateAssetMenu(fileName = "New Pause Menu Text Info", menuName = "Pause Menu Text Info", order = 8)]
public class PauseMenuTextInfo : ScriptableObject
{
    [Header("Common")]
    public string backButtonText;

    [Header("Main Screen")]
    public string pauseTitle;
    public string[] mainButtonTexts;

    [Header("Clues Screen")]
    public string cluesTitle;

    [Header("Inventory Screen")]
    public string inventoryTitle;
    
    [Header("Settings Screen")]
    public string settingsTitle;
    public string[] settingsOptions;
    public string[] textSpeedTexts;
}