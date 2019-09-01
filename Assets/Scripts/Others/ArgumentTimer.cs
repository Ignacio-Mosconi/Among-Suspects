using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ArgumentTimer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject timerPanel = default;
    [SerializeField] TextMeshProUGUI minutesText = default;
    [SerializeField] TextMeshProUGUI secondsText = default;
    [SerializeField] TextMeshProUGUI hundredthsOfSecondText = default;
    [Header("Time Values")]
    [SerializeField, Range(60f, 120f)] float baseAnswerTime = 120f;
    [SerializeField, Range(20f, 60f)] float warningTime = 60f;
    [SerializeField, Range(5f, 20f)] float criticalTime = 20f;
    [Header("Difficulty")]
    [SerializeField, Range(1, 3)] int maxDifficultyLevel = 3;
    [SerializeField, Range(1, 5)] int difficultyChangeIntervals = 3;
    [Header("Timer Colors")]
    [SerializeField] Color normalTimeColor = Color.white;
    [SerializeField] Color warningTimeColor = Color.yellow;
    [SerializeField] Color criticalTimeColor = Color.red;

    UnityEvent onTimeOut = new UnityEvent();

    float timer = 0f;
    float lastRemainingTimeOnStop = 0f;
    float lastAvailableAnsweringTime = 0f;
    int difficultyLevel = 1;
    int argumentsSinceLastDifficultyChange = 0;

    void Start()
    {
        enabled = false;
    }

    void Update()
    {
#if UNITY_EDITOR
    if (Input.GetKey(KeyCode.T))
        timer = 5f;
#endif

        timer -= Time.deltaTime;

        int minutesLeft = (int)timer / 60;
        int secondsLeft = (int)timer % 60;
        int hundredthsOfSecondLeft = (int)((timer - (int)timer) * 100f);

        string minutes = (minutesLeft >= 10) ? minutesLeft.ToString() : "0" + minutesLeft.ToString();
        string seconds = (secondsLeft >= 10) ? secondsLeft.ToString() : "0" + secondsLeft.ToString();
        string hundredthsOfSecond = (hundredthsOfSecondLeft >= 10) ? hundredthsOfSecondLeft.ToString() : 
                                                                    "0" + hundredthsOfSecondLeft.ToString();

        minutesText.text = minutes + "\"";
        secondsText.text = seconds + ".";
        hundredthsOfSecondText.text = hundredthsOfSecond + "'";

        if (timer < warningTime / difficultyLevel)
        {
            if (timer > criticalTime / difficultyLevel)
                CheckTextColorChange(warningTimeColor);
            else
                CheckTextColorChange(criticalTimeColor);
        }

        if (timer <= 0f)
            onTimeOut.Invoke();
    }

    void CheckTextColorChange(Color targetColor)
    {
        if (minutesText.color != targetColor)
        {
            minutesText.color = targetColor;
            secondsText.color = targetColor;
            hundredthsOfSecondText.color = targetColor;
        }
    }

    public void StartTimer()
    {
        timer = baseAnswerTime / difficultyLevel;
        lastAvailableAnsweringTime = timer;
        minutesText.color = normalTimeColor;
        secondsText.color = normalTimeColor;
        hundredthsOfSecondText.color = normalTimeColor;
        timerPanel.SetActive(true);
        enabled = true;
    }

    public void StopTimer()
    {
        lastRemainingTimeOnStop = Mathf.Max(timer, 0f);
        timerPanel.SetActive(false);
        enabled = false;
        argumentsSinceLastDifficultyChange++;
        if (argumentsSinceLastDifficultyChange == difficultyChangeIntervals)
        {
            argumentsSinceLastDifficultyChange = 0;
            if (difficultyLevel < maxDifficultyLevel)
                difficultyLevel++; 
        }
    }

    #region Properties

    public UnityEvent OnTimeOut
    {
        get { return onTimeOut; }
    }

    public float LastRemainingTimeOnStop
    {
        get { return lastRemainingTimeOnStop; }
    }

    public float LastAvailableAnsweringTime
    {
        get { return lastAvailableAnsweringTime; }
    }

    #endregion
}