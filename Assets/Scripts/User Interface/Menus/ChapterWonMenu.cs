using UnityEngine;
using TMPro;

public class ChapterWonMenu : Menu
{
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI debateResultsTitleText = default;
    [SerializeField] TextMeshProUGUI scoreText = default;
    [SerializeField] TextMeshProUGUI continueButtonText = default;
    [SerializeField] TextMeshProUGUI chapterFinishedTitleText = default;
    [SerializeField] TextMeshProUGUI nextChapterStartDescriptionText = default;
    [SerializeField] TextMeshProUGUI[] continueOptionsTexts = default;

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        ChapterWonMenuTextInfo chapterWonMenuTextInfo = menuTextsByLanguage[language] as ChapterWonMenuTextInfo;

        if (!chapterWonMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        debateResultsTitleText.text = chapterWonMenuTextInfo.debateResultsTitle;
        scoreText.text = chapterWonMenuTextInfo.score;
        continueButtonText.text = chapterWonMenuTextInfo.continueButtonText;
        chapterFinishedTitleText.text = chapterWonMenuTextInfo.chapterFinishedTitle;
        nextChapterStartDescriptionText.text = chapterWonMenuTextInfo.nextChapterStartDescription;
        for (int i = 0; i < continueOptionsTexts.Length; i++)
            continueOptionsTexts[i].text = chapterWonMenuTextInfo.continueOptions[i];
    }
}