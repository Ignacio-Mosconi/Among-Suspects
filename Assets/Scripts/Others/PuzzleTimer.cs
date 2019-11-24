using System.Collections;
using UnityEngine;
using TMPro;

public struct PuzzleTimespan
{
    public float totalTimeInSeconds;
    public int minutes;
    public int seconds;
    public int hundredthsOfSecond;
}

public class PuzzleTimer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject timerPanel = default;
    [SerializeField] TextMeshProUGUI minutesText = default;
    [SerializeField] TextMeshProUGUI secondsText = default;
    [SerializeField] TextMeshProUGUI hundredthsOfSecondText = default;

    [Header("Timer Animation")]
    [SerializeField, Range(1f, 1.5f)] float maxTimerScale = 1.2f;
    [SerializeField, Range(0.5f, 1f)] float minTimerScale = 0.8f;

    PuzzleTimespan timeToSolve;

    float timer = 0f;
    int totalMinutesElapsed = 0;

    const float ScaleDownDuration = 0.8f;
    const float ScaleUpDuration = 0.2f;
    const int MaxTimerMinutes = 60;

    void Start()
    {
        enabled = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        int minutesElapsed = (int)timer / 60;
        int secondsElapsed = (int)timer % 60;
        int hundredthsOfSecondElapsed = (int)((timer - (int)timer) * 100f);

        string minutes = minutesElapsed.ToString("00");
        string seconds = secondsElapsed.ToString("00");
        string hundredthsOfSecond = hundredthsOfSecondElapsed.ToString("00");

        minutesText.text = minutes + "'";
        secondsText.text = seconds + ".";
        hundredthsOfSecondText.text = hundredthsOfSecond + "\"";

        if (minutesElapsed > totalMinutesElapsed)
        {
            StartCoroutine(TickTimer());
            totalMinutesElapsed = minutesElapsed;
            if (totalMinutesElapsed == MaxTimerMinutes)
            {
                timerPanel.SetActive(false);
                enabled = false;
            }
        }
    }

    IEnumerator TickTimer()
    {
        float scaleTimer = 0f;
        float updatedTimerScale = 0f;
        float targetTimerScale = minTimerScale;

        while (scaleTimer < ScaleDownDuration)
        {
            scaleTimer += Time.deltaTime;
            updatedTimerScale = Mathf.SmoothStep(1f, targetTimerScale, scaleTimer / ScaleDownDuration);
            timerPanel.transform.localScale = new Vector3(updatedTimerScale, updatedTimerScale, updatedTimerScale);

            yield return new WaitForEndOfFrame();
        }

        scaleTimer = 0f;
        targetTimerScale = maxTimerScale;

        while (scaleTimer < ScaleUpDuration)
        {
            scaleTimer += Time.deltaTime;
            updatedTimerScale = Mathf.Lerp(1f, targetTimerScale, scaleTimer / ScaleUpDuration);
            timerPanel.transform.localScale = new Vector3(updatedTimerScale, updatedTimerScale, updatedTimerScale);

            yield return new WaitForEndOfFrame();
        }

        AudioManager.Instance.PlaySound("Clock Tick");

        scaleTimer = 0f;
        targetTimerScale = 1f;

        while (scaleTimer < ScaleDownDuration)
        {
            scaleTimer += Time.deltaTime;
            updatedTimerScale = Mathf.SmoothStep(1f, targetTimerScale, scaleTimer / ScaleDownDuration);
            timerPanel.transform.localScale = new Vector3(updatedTimerScale, updatedTimerScale, updatedTimerScale);

            yield return new WaitForEndOfFrame();
        }
    }

    public void StartTimer()
    {
        timer = 0f;
        timerPanel.SetActive(true);
        enabled = true;
    }

    public void StopTimer()
    {
        timeToSolve.totalTimeInSeconds = timer;
        timeToSolve.minutes = (int)timer / 60;
        timeToSolve.seconds = (int)timer % 60;
        timeToSolve.hundredthsOfSecond = (int)((timer - (int)timer) * 100f);

        timer = 0f;
        enabled = false;
    }

    #region Properties

    public PuzzleTimespan TimeToSolve
    {
        get { return timeToSolve; }
    }

    #endregion
}