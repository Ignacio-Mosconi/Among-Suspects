using UnityEngine;

[CreateAssetMenu(fileName = "New Chapter Won Menu Text Info", menuName = "Chapter Won Menu Text Info", order = 10)]
public class ChapterWonMenuTextInfo : ScriptableObject
{
    [Header("Score Screen")]
    public string debateResultsTitle = default;
    public string score = default;
    public string continueButtonText = default;

    [Header("Chapter Finished Screen")]
    public string chapterFinishedTitle = default;
    public string nextChapterStartDescription = default;
    public string[] continueOptions = default;
}