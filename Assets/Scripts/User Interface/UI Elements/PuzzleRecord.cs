using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(UIPrompt))]
public class PuzzleRecord : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI minutesTakenText = default;
    [SerializeField] TextMeshProUGUI secondsTakenText = default;
    [SerializeField] TextMeshProUGUI hundrethsOfSecondTakenText = default;
    [SerializeField] TextMeshProUGUI timeToSolveText = default;
    [SerializeField] TextMeshProUGUI tierText = default;
    [SerializeField] string[] timeToSolveTextByLanguage = new string[(int)Language.Count];

    PuzzleRecordData puzzleRecordData;
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

    public void UpdateRecordData(PuzzleRecordData puzzleRecordData, ref Color[] tierColors)
    {
        this.puzzleRecordData = puzzleRecordData;

        minutesTakenText.text = puzzleRecordData.timeToSolve.minutes.ToString("00") + "'";
        secondsTakenText.text = puzzleRecordData.timeToSolve.minutes.ToString("00") + ".";
        hundrethsOfSecondTakenText.text = puzzleRecordData.timeToSolve.hundredthsOfSecond.ToString("00") + "\"";

        tierText.text = Enum.GetName(typeof(PuzzleSolvingTier), puzzleRecordData.tier);
        tierText.color = tierColors[(int)puzzleRecordData.tier];

        Language language = GameManager.Instance.CurrentLanguage;

        timeToSolveText.text = timeToSolveTextByLanguage[(int)language] + ": ";
    }

    #region Properties

    public PuzzleRecordData PuzzleRecordData
    {
        get { return puzzleRecordData; }
    }

    public UIPrompt UIPrompt
    {
        get { return uiPrompt; }
    }

    #endregion
}