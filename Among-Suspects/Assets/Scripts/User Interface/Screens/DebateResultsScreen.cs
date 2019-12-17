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
    [SerializeField] Color[] puzzleTierColors = new Color[(int)PuzzleSolvingTier.Count];

    List<ArgumentRecord> argumentRecords = new List<ArgumentRecord>();
    List<StarRating> starRatings = new List<StarRating>();
    PuzzleRecord puzzleRecord;
    GameObject argumentRecordPrefab;
    GameObject puzzleRecordPrefab;
    GameObject starRatingPrefab;
    Coroutine scoreDisplayingRoutine;
    int scoreValue = 0;
    int starsToDisplay = 0;

    const int ScoreAdditionIncrements = 5;
    const float DelayBetweenArgumentReviews = 1f;
    const string ArgumentRecordPrefabPath = "Menu Elements/Argument Record";
    const string PuzzleRecordPrefabPath = "Menu Elements/Puzzle Record";
    const string StarRatingPrefabPath = "Menu Elements/Star Rating";

    void Awake()
    {
        argumentRecordPrefab = Resources.Load(ArgumentRecordPrefabPath) as GameObject;
        puzzleRecordPrefab = Resources.Load(PuzzleRecordPrefabPath) as GameObject;
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
        
        PuzzleRecordData puzzleRecordData = debatePerformanceController.PuzzleRecordData;
        GameObject puzzleRecordObject = Instantiate(puzzleRecordPrefab, resultsPanel);
        
        puzzleRecord = puzzleRecordObject.GetComponent<PuzzleRecord>();

        puzzleRecord.SetUpPrompt();
        puzzleRecord.UpdateRecordData(puzzleRecordData, ref puzzleTierColors);
        puzzleRecord.gameObject.SetActive(false);
    }

    void Start()
    {
        scoreDisplayingRoutine = StartCoroutine(DisplayScore());
    }

    void Update()
    {
        if (Input.GetButtonDown("Continue"))
            if (scoreDisplayingRoutine != null)
            {
                StopCoroutine(scoreDisplayingRoutine);
                scoreDisplayingRoutine = null;
                SkipScoreDisplay();
                enabled = false;
            }
    }

    void SkipScoreDisplay()
    {
        foreach (ArgumentRecord argumentRecord in argumentRecords)
            if (!argumentRecord.gameObject.activeInHierarchy)
            {
                scoreValue += argumentRecord.ArgumentRecordData.argumentScore;
                argumentRecord.ShowRecord();
            }

        if (!puzzleRecord.gameObject.activeInHierarchy)
        {
            scoreValue += puzzleRecord.PuzzleRecordData.puzzleScore;
            puzzleRecord.ShowRecord();
        }

        scoreAmountText.text = scoreValue.ToString();

        for (int i = 0; i < starsToDisplay; i++)
            starRatings[i].ShowStar();

        continueButtonPrompt.Show();
    }

    IEnumerator DisplayScore()
    {
        int addedScore = 0;

        foreach (ArgumentRecord argumentRecord in argumentRecords)
        {
            addedScore = argumentRecord.ArgumentRecordData.argumentScore;
            argumentRecord.ShowRecord();

            yield return new WaitForSeconds(argumentRecord.UIPrompt.ShowAnimationDuration);

            yield return StartCoroutine(IncreaseScoreGradually(addedScore));
        }

        addedScore = puzzleRecord.PuzzleRecordData.puzzleScore;
        puzzleRecord.ShowRecord();

        yield return new WaitForSeconds(puzzleRecord.UIPrompt.ShowAnimationDuration);

        yield return StartCoroutine(ShowStarsGradually());

        continueButtonPrompt.Show();

        scoreDisplayingRoutine = null;
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