using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UIPrompt))]
public class ArgumentRecord : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI argumentNumberText = default;
    [SerializeField] TextMeshProUGUI timeLeftAmountText = default;
    [SerializeField] Image resultIcon = default;
    [SerializeField] TextMeshProUGUI resultText = default;
    [SerializeField] TextMeshProUGUI timeLeftText = default;
    [SerializeField] string[] resultTextByLanguage = new string[(int)Language.Count];
    [SerializeField] string[] timeLeftTextByLanguage = new string[(int)Language.Count];

    ArgumentRecordData argumentRecordData;
    UIPrompt uiPrompt;

    public void SetUpPrompt()
    {
        uiPrompt = GetComponent<UIPrompt>();
        uiPrompt.SetUp();
    }

    public void ShowRecord()
    {
        uiPrompt.Show();
    }

    public void UpdateRecordData(ArgumentRecordData argumentRecordData, int argumentNumber, ref Sprite[] resultSprites)
    {
        this.argumentRecordData = argumentRecordData;
        
        argumentNumberText.text = "#" + argumentNumber + ":";
        timeLeftAmountText.text = (int)argumentRecordData.timeLeftToSolve + "\"";
        resultIcon.sprite = (argumentRecordData.wasSolvedCorrectly) ? resultSprites[0] : resultSprites[1];

        Language language = GameManager.Instance.CurrentLanguage;

        resultText.text = resultTextByLanguage[(int)language] + ": ";
        timeLeftText.text = timeLeftTextByLanguage[(int)language] + ": ";
    }

    #region Properties

    public ArgumentRecordData ArgumentRecordData
    {
        get { return argumentRecordData; }
    }

    public UIPrompt UIPrompt
    {
        get { return uiPrompt; }
    }

    #endregion
}