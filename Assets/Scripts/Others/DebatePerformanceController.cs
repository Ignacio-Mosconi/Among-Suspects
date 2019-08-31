using System.Collections.Generic;
using UnityEngine;

public struct ArgumentRecord
{
    public bool wasSolvedCorrectly;
    public float timeLeftToSolve;
}

[System.Serializable]
public class DebatePerformanceController
{
    [SerializeField, Range(30f, 60f)] float initialCredibility = 50f;
    [SerializeField, Range(50f, 100f)] float requiredCredibility = 50f;
    
    List<ArgumentRecord> argumentRecords;
    int debateArguments;
    float credibility;
    float credibilityIncreaseRate;
    float credibilityDecreaseRate;

    public DebatePerformanceController()
    {
        argumentRecords = new List<ArgumentRecord>();
    }

    public void Initialize(int numberOfArguments)
    {
        argumentRecords.Clear();
        debateArguments = numberOfArguments;
        argumentRecords.Capacity = debateArguments;

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

    public void IncreaseCredibility()
    {
        credibility += credibilityIncreaseRate;
    }

    public void DecreaseCredibility()
    {
        credibility -= credibilityDecreaseRate;
    }

    #region Properties

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

    #endregion
}