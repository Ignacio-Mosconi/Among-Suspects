using UnityEngine;

[CreateAssetMenu(fileName = "New Main Menu Text Info", menuName = "Main Menu Text Info", order = 7)]
public class MainMenuTextInfo : ScriptableObject
{
    [Header("Common")]
    public string backButtonText;

    [Header("Main Screen")]
    public string[] mainButtonTexts;
    
    [Header("Controls Screen")]
    public string controlsTitle;
    public string[] controlsTexts;
    
    [Header("Settings Screen")]
    public string settingsTitle;
    public string[] settingsOptions;
    public string[] textSpeedTexts;

    [Header("Credits Screen")]
    public string creditsTitle;
    public string thanksText;
}