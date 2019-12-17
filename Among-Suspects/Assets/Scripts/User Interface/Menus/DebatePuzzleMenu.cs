using UnityEngine;
using TMPro;

public class DebatePuzzleMenu : Menu
{
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI solveTheCaseText = default;
    [SerializeField] TextMeshProUGUI[] instructionTexts = default;
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

        solveTheCaseText.text = debatePuzzleMenuTextInfo.solveTheCaseTitleText;
        for (int i = 0; i < instructionTexts.Length; i++)
            instructionTexts[i].text = debatePuzzleMenuTextInfo.instructionTexts[i];
        finalArgumentText.text = debatePuzzleMenuTextInfo.finalArgumentButtonText;
    }
}