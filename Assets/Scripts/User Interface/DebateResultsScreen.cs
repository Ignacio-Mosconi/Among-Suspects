using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AnimatedMenuScreen))]
public class DebateResultsScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreAmountText = default;
    [SerializeField] UIPrompt continueButtonPrompt = default;
    [SerializeField] Transform resultsPanel = default;
    [SerializeField] Transform starsPanel = default;
    [SerializeField] Sprite[] resultIconSprites = new Sprite[2];

    List<ArgumentRecord> argumentRecords = new List<ArgumentRecord>();
    List<StarRating> starRatings = new List<StarRating>();
    GameObject argumentRecordPrefab;
    GameObject starRatingPrefab;
    int scoreValue = 0;
    int starsToDisplay = 0;

    const int ScoreAdditionIncrements = 5;
    const float DelayBetweenArgumentReviews = 1f;
    const string ArgumentRecordPrefabPath = "Menu Elements/Argument Record";
    const string StarRatingPrefabPath = "Menu Elements/Star Rating";

    void Awake()
    {
        argumentRecordPrefab = Resources.Load(ArgumentRecordPrefabPath) as GameObject;
        starRatingPrefab = Resources.Load(StarRatingPrefabPath) as GameObject;

        continueButtonPrompt.SetUp();

        DebatePerformanceController debatePerformanceController = DebateManager.Instance.DebatePerformanceController;

        debatePerformanceController.ComputeStarRatings();

        starsToDisplay = debatePerformanceController.ScoreRecordData.starsAchieved;

        for (int i = 0; i < debatePerformanceController.StarRatings; i++)
        {
            GameObject starRatingObject = Instantiate(starRatingPrefab, starsPanel);
            StarRating starRating = starRatingObject.GetComponent<StarRating>();

            starRating.SetUpPrompt();
            starRating.HideStar();
            
            starRatings.Add(starRating);
        }
        
        List<ArgumentRecordData> argumentRecordsData = debatePerformanceController.ArgumentRecordsData;

        int argumentNumber = 1;
        foreach (ArgumentRecordData argumentRecordData in argumentRecordsData)
        {
            GameObject argumentRecordObject = Instantiate(argumentRecordPrefab, resultsPanel);
            ArgumentRecord argumentRecord = argumentRecordObject.GetComponent<ArgumentRecord>();

            argumentRecord.SetUpPrompt();
            argumentRecord.UpdateRecordData(argumentRecordData, argumentNumber, ref resultIconSprites);

            argumentRecord.gameObject.SetActive(false);
            argumentNumber++;

            argumentRecords.Add(argumentRecord);
        }
    }

    IEnumerator Start()
    {
        foreach (ArgumentRecord argumentRecord in argumentRecords)
        {
            int addedScore = argumentRecord.ArgumentRecordData.argumentScore;      
            argumentRecord.ShowRecord();
            
            yield return new WaitForSeconds(argumentRecord.UIPrompt.ShowAnimationDuration);
            
            yield return StartCoroutine(IncreaseScoreGradually(addedScore));
        }

        yield return StartCoroutine(ShowStarsGradually());

        continueButtonPrompt.Show();
    }

    IEnumerator IncreaseScoreGradually(int addedScore)
    {
        int currentScoreValue = scoreValue;
        scoreValue += addedScore;
        
        while (currentScoreValue < scoreValue)
        {
            currentScoreValue = Mathf.Min(currentScoreValue + ScoreAdditionIncrements, scoreValue);
            scoreAmountText.text = currentScoreValue.ToString();

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ShowStarsGradually()
    {
        int starsIndex = 0;

        while (starsIndex < starsToDisplay)
        {
            starRatings[starsIndex].ShowStar();

            yield return new WaitForSeconds(starRatings[starsIndex].UIPrompt.ShowAnimationDuration);
            
            starsIndex++;
        }
    }
}