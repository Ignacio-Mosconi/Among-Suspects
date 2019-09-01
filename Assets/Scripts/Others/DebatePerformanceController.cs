using System.Collections.Generic;
using UnityEngine;

public struct ArgumentRecordData
{
    public bool wasSolvedCorrectly;
    public float timeLeftToSolve;
    public float totalTimeToSolve;
    public int argumentScore;
}

public struct ScoreRecordData
{
    public int scoreAchieved;
    public int starsAchieved;
}

[System.Serializable]
public class DebatePerformanceController
{
    [SerializeField, Range(30f, 60f)] float initialCredibility = 50f;
    [SerializeField, Range(50f, 100f)] float requiredCredibility = 50f;
    [SerializeField, Range(50f, 100f)] float timeLeftPercentageForMaxScore = 50f;
    [SerializeField, Range(50, 100)] int scorePerCorrectReaction = 50;
    [SerializeField, Range(3, 5)] int starRatings = 5;
    
    List<ArgumentRecordData> argumentRecordsData;
    ScoreRecordData scoreRecordData;
    int debateArguments;
    float credibility;
    float credibilityIncreaseRate;
    float credibilityDecreaseRate;

    public DebatePerformanceController()
    {
        argumentRecordsData = new List<ArgumentRecordData>();
    }

    void RegisterArgumentRecord(bool wasSolvedCorrectly, float timeLeftToSolve, float totalTimeToSolve)
    {
        if (argumentRecordsData.Count >= debateArguments)
        {
            Debug.LogError("All the records have been stored for this debate.");
            return;
        }
        
        float timeLeftPerc = timeLeftToSolve * 100f / totalTimeToSolve;
        int argumentScore = (wasSolvedCorrectly) ? scorePerCorrectReaction + (int)timeLeftPerc : -(int)timeLeftPerc;

        ArgumentRecordData argumentRecord;

        argumentRecord.wasSolvedCorrectly = wasSolvedCorrectly;
        argumentRecord.timeLeftToSolve = timeLeftToSolve;
        argumentRecord.totalTimeToSolve = totalTimeToSolve;
        argumentRecord.argumentScore = argumentScore;

        argumentRecordsData.Add(argumentRecord);
    }

    public void Initialize(int numberOfArguments)
    {
        argumentRecordsData.Clear();
        debateArguments = numberOfArguments;
        argumentRecordsData.Capacity = debateArguments;

        credibility = initialCredibility;
        credibilityIncreaseRate = (100f - initialCredibility) / numberOfArguments;
        credibilityDecreaseRate = initialCredibility / numberOfArguments;
    }

    public bool ShouldLoseCase(int currentArgumentIndex)
    {
        bool shouldLose = false;
        
        int argumentsRemaining = debateArguments - currentArgumentIndex - 1;
        float maxAchievableCredibility = credibility + argumentsRemaining * credibilityIncreaseRate;

        shouldLose = (maxAchievableCredibility < requiredCredibility);

        return shouldLose;
    }

    public bool IsAtCriticalCredibility(int currentArgumentIndex)
    {
        bool isAtCriticalCredibility = false;

        int argumentsRemainingAfterNext = debateArguments - (currentArgumentIndex + 2);
        float credibilityAtNextFail = credibility - credibilityDecreaseRate;
        
        isAtCriticalCredibility = (credibilityAtNextFail + credibilityIncreaseRate * argumentsRemainingAfterNext <= requiredCredibility);

        return isAtCriticalCredibility;
    }

    public void IncreaseCredibility(float timeLeftToSolve, float totalTimeToSolve)
    {
        credibility += credibilityIncreaseRate;
        RegisterArgumentRecord(wasSolvedCorrectly: true, timeLeftToSolve, totalTimeToSolve);
    }

    public void DecreaseCredibility(float timeLeftToSolve, float totalTimeToSolve)
    {
        credibility -= credibilityDecreaseRate;
        RegisterArgumentRecord(wasSolvedCorrectly: false, timeLeftToSolve, totalTimeToSolve);
    }

    public void ComputeStarRatings()
    {
        int scorePerStar = 0;
        int scoreAchieved = 0;
        int maxQualificationScore = (scorePerCorrectReaction + (int)timeLeftPercentageForMaxScore) * argumentRecordsData.Count;

        foreach (ArgumentRecordData argumentRecordData in argumentRecordsData)
            scoreAchieved += argumentRecordData.argumentScore;
        
        scorePerStar = maxQualificationScore / starRatings;
        
        scoreRecordData.scoreAchieved = Mathf.Max(0, scoreAchieved);
        scoreRecordData.starsAchieved = Mathf.Clamp(scoreAchieved / scorePerStar, 1, starRatings);
    }

    #region Properties

    public List<ArgumentRecordData> ArgumentRecordsData
    {
        get { return argumentRecordsData; }
    }

    public ScoreRecordData ScoreRecordData
    {
        get { return scoreRecordData; }
    }

    public float InitialCredibility
    {
        get { return initialCredibility; }
    }

    public float RequiredCredibility
    {
        get { return requiredCredibility; }
    }

    public float Credibility
    {
        get { return credibility; }
    }

    public int DebateArguments
    {
        get { return debateArguments; }
    }

    public int StarRatings
    {
        get { return starRatings; }
    }

    #endregion
}