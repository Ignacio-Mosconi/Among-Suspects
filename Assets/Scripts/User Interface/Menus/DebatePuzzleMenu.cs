using UnityEngine;
using TMPro;

public class DebatePuzzleMenu : Menu
{
    [Header("Translation Texts")]
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
        finalArgumentText.text = debatePuzzleMenuTextInfo.finalArgumentButtonText;
    }
}