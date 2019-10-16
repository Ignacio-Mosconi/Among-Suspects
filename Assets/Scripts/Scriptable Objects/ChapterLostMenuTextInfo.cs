using UnityEngine;

[CreateAssetMenu(fileName = "New Chapter Lost Menu Text Info", menuName = "Chapter Lost Menu Text Info", order = 11)]
public class ChapterLostMenuTextInfo : ScriptableObject
{
    [Header("Chapter Lost Screen")]
    public string debateLostTitle = default;
    public string retryDescription = default;
    public string[] continueOptions = default;
}