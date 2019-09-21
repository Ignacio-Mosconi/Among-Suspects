using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CredibilityBarController : MonoBehaviour
{
    [SerializeField] GameObject credibilityPanel = default;
    [SerializeField] Image credibilityBar = default;
    [SerializeField] Image credibilityBarBackground = default;
    [SerializeField] Image credibilityIcon = default;
    [SerializeField] [Range(1f, 3f)] float fillBarDuration = 1.5f;
    [SerializeField] [Range(1f, 2f)] float idleBarDuration = 1.5f;
    [SerializeField] [Range(0.5f, 1f)] float fadingDuration = 0.75f;
    [SerializeField] [Range(0.25f, 0.5f)] float scaleDuration = 0.4f;
    [SerializeField] [Range(1f, 1.3f)] float maxIconScale = 1.2f;
    [SerializeField] [Range(0.7f, 1f)] float minIconScale = 0.8f;
    [SerializeField] Sprite[] credibilitySprites = default;

    Coroutine fillingBarRoutine;

    void ScaleCredibilityIcon(ref bool scaleUp, ref float scaleTimer)
    {
        float newIconScale = (scaleUp) ? Mathf.SmoothStep(1f, maxIconScale, scaleTimer / scaleDuration) :
                                            Mathf.SmoothStep(1f, minIconScale, scaleTimer / scaleDuration);

        credibilityIcon.transform.localScale = new Vector3(newIconScale, newIconScale, newIconScale);

        if (scaleTimer >= scaleDuration)
        {
            scaleTimer = 0f;
            scaleUp = !scaleUp;
        }
    }

    IEnumerator FillBar(float credibilityPerc, float minPercRequired, bool isCriticalPerc)
    {
        float timer = 0f;
        float scaleTimer = 0f;
        float currentFill = credibilityBar.fillAmount;
        float targetFill = credibilityPerc / 100f;
        bool isIncreasingIconSize = true;
        
        Color newBarColor = credibilityBar.color;
        Color newBackgroundColor = credibilityBarBackground.color;
        Color newCredibilityIconColor = credibilityIcon.color;
        newBarColor.a = newBackgroundColor.a = newCredibilityIconColor.a = 0f;

        credibilityIcon.sprite = (targetFill > currentFill) ? credibilitySprites[0] : credibilitySprites[1];

        credibilityPanel.SetActive(true);

        while (timer < fillBarDuration)
        {
            timer += Time.deltaTime;
            scaleTimer += Time.deltaTime;

            credibilityBar.fillAmount = Mathf.Lerp(currentFill, targetFill, timer / fillBarDuration);

            ScaleCredibilityIcon(ref isIncreasingIconSize, ref scaleTimer);

            float currentPerc = credibilityBar.fillAmount * 100f;

            newBarColor.a = newBackgroundColor.a = newCredibilityIconColor.a = Mathf.Lerp(0f, 1f, timer / fadingDuration);

            if (newBarColor != credibilityBar.color)
                credibilityBar.color = newBarColor;
            if (newBackgroundColor != credibilityBarBackground.color)
                credibilityBarBackground.color = newBackgroundColor;
            if (newCredibilityIconColor != credibilityIcon.color)
                credibilityIcon.color = newCredibilityIconColor;

            yield return new WaitForEndOfFrame();
        }

        timer = 0f;

        while (timer < idleBarDuration)
        {
            timer += Time.deltaTime;
            scaleTimer += Time.deltaTime;

            ScaleCredibilityIcon(ref isIncreasingIconSize, ref scaleTimer); 

            yield return new WaitForEndOfFrame();
        }

        timer = 0f;

        float previousIconScale = credibilityIcon.transform.localScale.x;

        while (timer < fadingDuration)
        {
            timer += Time.deltaTime;

            float newIconScale = Mathf.SmoothStep(previousIconScale, 1f, timer / fadingDuration);
            
            credibilityIcon.transform.localScale = new Vector3(newIconScale, newIconScale, newIconScale);
            newBarColor.a = newBackgroundColor.a = newCredibilityIconColor.a = Mathf.Lerp(1f, 0f, timer / fadingDuration);

            credibilityBar.color = newBarColor;
            credibilityBarBackground.color = newBackgroundColor;
            credibilityIcon.color = newCredibilityIconColor;

            yield return new WaitForEndOfFrame();
        }

        credibilityPanel.SetActive(false);

        fillingBarRoutine = null;
    }

    public void StartFillingBar(float credibilityPerc, float minPercRequired, bool isCriticalPerc)
    {
        fillingBarRoutine = StartCoroutine(FillBar(credibilityPerc, minPercRequired, isCriticalPerc));
    }

    public void StopFillingBar()
    {
        if (fillingBarRoutine != null)
        {
            StopCoroutine(fillingBarRoutine);
            fillingBarRoutine = null;
        }
    }

    public bool IsFillingBar()
    {
        return (fillingBarRoutine != null);
    }

    public void ResetCredibilityBar(float credibilityPerc)
    {
        credibilityBar.fillAmount = credibilityPerc / 100f;
        credibilityIcon.sprite = credibilitySprites[1];
        credibilityIcon.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}