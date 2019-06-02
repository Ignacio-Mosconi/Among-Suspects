using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CredibilityBarController : MonoBehaviour
{
    [SerializeField] GameObject credibilityPanel = default;
    [SerializeField] Image credibilityBar = default;
    [SerializeField] Image credibilityIcon = default;
    [SerializeField] [Range(0.5f, 1.5f)] float fillBarDuration = 1f;
    [SerializeField] [Range(2f, 4f)] float idleBarDuration = 3f;
    [SerializeField] Color colorPositive = Color.green;
    [SerializeField] Color colorNeutral = Color.yellow;
    [SerializeField] Color colorNegative = Color.red;
    [SerializeField] Sprite[] credibilitySprites = default;

    Coroutine fillingBarRoutine;

    IEnumerator FillBar(float credibilityPerc, float minPercRequired, bool isCriticalPerc)
    {
        float timer = 0f;
        float currentFill = credibilityBar.fillAmount;
        float targetFill = credibilityPerc / 100f;

        credibilityPanel.SetActive(true);

        while (credibilityBar.fillAmount != targetFill)
        {
            timer += Time.deltaTime;
            credibilityBar.fillAmount = Mathf.Lerp(currentFill, targetFill, timer / fillBarDuration);

            Color newColor = credibilityBar.color;
            float currentPerc = credibilityBar.fillAmount * 100f;

            if (currentPerc >= minPercRequired && credibilityBar.color != colorPositive)
            {
                newColor = colorPositive;
                credibilityIcon.sprite = credibilitySprites[0];
            }

            if (currentPerc < minPercRequired && !isCriticalPerc && credibilityBar.color != colorNeutral)
            {
                newColor = colorNeutral;
                credibilityIcon.sprite = credibilitySprites[1];
            }

            if (isCriticalPerc && credibilityBar.color != colorNegative)
            {
                newColor = colorNegative;
                credibilityIcon.sprite = credibilitySprites[2];
            }

            if (newColor != credibilityBar.color)
            {
                newColor.a = credibilityBar.color.a;
                credibilityBar.color = newColor;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(idleBarDuration);

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
        credibilityBar.color = colorNeutral;
        credibilityIcon.sprite = credibilitySprites[1];
    }
}