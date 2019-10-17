using UnityEngine;
using TMPro;

public class ClueOptionsMenu : Menu
{
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI selectEvidenceTitleText = default;
    [SerializeField] TextMeshProUGUI backButtonText = default;
    [SerializeField] TextMeshProUGUI useButtonText = default;

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        ClueOptionsMenuTextInfo clueOptionsMenuTextInfo = menuTextsByLanguage[language] as ClueOptionsMenuTextInfo;

        if (!clueOptionsMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        selectEvidenceTitleText.text = clueOptionsMenuTextInfo.selectEvidenceTitle;
        backButtonText.text = clueOptionsMenuTextInfo.backButtonText;
        useButtonText.text = clueOptionsMenuTextInfo.useButtonText;
    }
}