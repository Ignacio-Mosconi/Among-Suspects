using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UIPrompt))]
public class ArgumentRecord : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI argumentNumberText = default;
    [SerializeField] TextMeshProUGUI timeLeftAmountText = default;
    [SerializeField] Image resultIcon = default;

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