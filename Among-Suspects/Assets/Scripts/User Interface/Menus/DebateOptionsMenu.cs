using UnityEngine;
using TMPro;

public class DebateOptionsMenu : Menu
{
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI trustText = default;
    [SerializeField] TextMeshProUGUI refuteText = default;

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        DebateOptionsMenuTextInfo debateOptionsMenuTextInfo = menuTextsByLanguage[language] as DebateOptionsMenuTextInfo;

        if (!debateOptionsMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        trustText.text = debateOptionsMenuTextInfo.trust;
        refuteText.text = debateOptionsMenuTextInfo.refute;
    }
}