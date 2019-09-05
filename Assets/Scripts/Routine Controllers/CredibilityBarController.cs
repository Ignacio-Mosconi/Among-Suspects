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
    [SerializeField] Sprite[] credibilitySprites = default;

    Coroutine fillingBarRoutine;

    IEnumerator FillBar(float credibilityPerc, float minPercRequired, bool isCriticalPerc)
    {
        float timer = 0f;
        float currentFill = credibilityBar.fillAmount;
        float targetFill = credibilityPerc / 100f;
        
        Color newBarColor = credibilityBar.color;
        Color newBackgroundColor = credibilityBarBackground.color;
        Color newCredibilityIconColor = credibilityIcon.color;
        newBarColor.a = newBackgroundColor.a = newCredibilityIconColor.a = 0f;

        credibilityIcon.sprite = (targetFill > currentFill) ? credibilitySprites[0] : credibilitySprites[1];

        credibilityPanel.SetActive(true);

        while (timer < fillBarDuration)
        {
            timer += Time.deltaTime;
            credibilityBar.fillAmount = Mathf.Lerp(currentFill, targetFill, timer / fillBarDuration);

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

        yield return new WaitForSeconds(idleBarDuration);

        timer = 0f;

        while (timer < fadingDuration)
        {
            timer += Time.deltaTime;
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
    }
}