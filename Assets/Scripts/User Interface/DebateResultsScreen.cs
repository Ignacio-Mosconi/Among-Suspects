using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebateResultsScreen : MonoBehaviour
{
    [SerializeField] GridLayoutGroup resultsPanel;
    [SerializeField] Sprite[] resultIconSprites = new Sprite[2];

    GameObject argumentRecordPrefab;

    const string ArgumentRecordPrefabPath = "Menu Elements/Argument Record";

    void Awake()
    {
        argumentRecordPrefab = Resources.Load(ArgumentRecordPrefabPath) as GameObject;

        DebatePerformanceController debatePerformanceController = DebateManager.Instance.DebatePerformanceController;

        debatePerformanceController.ComputeStarRatings();
        
        List<ArgumentRecordData> argumentRecordsData = debatePerformanceController.ArgumentRecordsData;
        int numberOfArguments = debatePerformanceController.DebateArguments;

        for (int i = 0; i < numberOfArguments; i++)
        {
            GameObject argumentRecordObject = Instantiate(argumentRecordPrefab, resultsPanel.transform);
            ArgumentRecordUI argumentRecordUI = argumentRecordObject.GetComponent<ArgumentRecordUI>();

            Sprite resultIconSprite = (argumentRecordsData[i].wasSolvedCorrectly) ? resultIconSprites[0] : resultIconSprites[1];

            argumentRecordUI.SetArgumentNumberText(i + 1);
            argumentRecordUI.SetTimeLeftText(argumentRecordsData[i].timeLeftToSolve);
            argumentRecordUI.SetResultIcon(resultIconSprite);
        }

        Debug.Log(debatePerformanceController.ScoreRecordData.scoreAchieved);
        Debug.Log(debatePerformanceController.ScoreRecordData.starsAchieved);
    }
}