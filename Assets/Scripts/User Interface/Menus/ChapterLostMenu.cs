using UnityEngine;
using TMPro;

public class ChapterLostMenu : Menu
{
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI guiltyContinueButtonText = default;
    [SerializeField] TextMeshProUGUI debateLostTitleText = default;
    [SerializeField] TextMeshProUGUI retryDescriptionText = default;
    [SerializeField] TextMeshProUGUI[] continueOptionsTexts = default;

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        ChapterLostMenuTextInfo chapterLostMenuTextInfo = menuTextsByLanguage[language] as ChapterLostMenuTextInfo;

        if (!chapterLostMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        guiltyContinueButtonText.text = chapterLostMenuTextInfo.guiltyContuneButtonText;
        debateLostTitleText.text = chapterLostMenuTextInfo.debateLostTitle;
        retryDescriptionText.text = chapterLostMenuTextInfo.retryDescription;
        for (int i = 0; i < continueOptionsTexts.Length; i++)
            continueOptionsTexts[i].text = chapterLostMenuTextInfo.continueOptions[i];
    }
}