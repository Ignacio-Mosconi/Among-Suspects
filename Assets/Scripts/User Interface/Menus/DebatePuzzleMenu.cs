using UnityEngine;
using TMPro;

public class DebatePuzzleMenu : Menu
{
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI puzzleLostTitleText = default;
    [SerializeField] TextMeshProUGUI retryDescriptionText = default;
    [SerializeField] TextMeshProUGUI[] continueOptionsTexts = default;
    [SerializeField] TextMeshProUGUI finalArgumentText = default;

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        DebatePuzzleMenuTextInfo debatePuzzleMenuTextInfo = menuTextsByLanguage[language] as DebatePuzzleMenuTextInfo;

        if (!debatePuzzleMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        puzzleLostTitleText.text = debatePuzzleMenuTextInfo.puzzleLostTitle;
        retryDescriptionText.text = debatePuzzleMenuTextInfo.retryDescription;
        for (int i = 0; i < continueOptionsTexts.Length; i++)
            continueOptionsTexts[i].text = debatePuzzleMenuTextInfo.continueOptions[i];
        finalArgumentText.text = debatePuzzleMenuTextInfo.finalArgumentButtonText;
    }
}