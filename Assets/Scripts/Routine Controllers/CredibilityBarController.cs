using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CredibilityBarController : MonoBehaviour
{
    [SerializeField] CanvasGroup credibilityPanel = default;
    [SerializeField] Image credibilityBar = default;
    [SerializeField] Image credibilityBarOutline = default;
    [SerializeField] Image credibilityIcon = default;
    [SerializeField] [Range(1f, 3f)] float fillBarDuration = 1.5f;
    [SerializeField] [Range(1f, 2f)] float idleBarDuration = 1.5f;
    [SerializeField] [Range(0.5f, 1f)] float fadingDuration = 0.75f;
    [SerializeField] [Range(0.25f, 0.5f)] float iconScaleDuration = 0.4f;
    [SerializeField] [Range(0.2f, 0.75f)] float outlineFlashIntervals = 0.3f;
    [SerializeField] [Range(1f, 1.3f)] float maxIconScale = 1.2f;
    [SerializeField] [Range(0.7f, 1f)] float minIconScale = 0.8f;
    [SerializeField] Sprite[] credibilitySprites = default;

    Coroutine fillingBarRoutine;

    void ScaleCredibilityIcon(ref bool scaleUp, ref float scaleTimer)
    {
        float newIconScale = (scaleUp) ? Mathf.SmoothStep(1f, maxIconScale, scaleTimer / iconScaleDuration) :
                                            Mathf.SmoothStep(1f, minIconScale, scaleTimer / iconScaleDuration);

        credibilityIcon.transform.localScale = new Vector3(newIconScale, newIconScale, newIconScale);

        if (scaleTimer >= iconScaleDuration)
        {
            scaleTimer = 0f;
            scaleUp = !scaleUp;
        }
    }

    IEnumerator FillBar(float credibilityPerc)
    {
        float timer = 0f;
        float scaleTimer = 0f;
        float outlineFlashTimer = 0f;
        float currentFill = credibilityBar.fillAmount;
        float targetFill = credibilityPerc / 100f;
        bool isIncreasingIconSize = true;

        credibilityIcon.sprite = (targetFill > currentFill) ? credibilitySprites[0] : credibilitySprites[1];
        credibilityBarOutline.fillAmount = 0f;
        
        credibilityPanel.gameObject.SetActive(true);
        credibilityBarOutline.gameObject.SetActive(true);

        while (timer < fillBarDuration)
        {
            timer += Time.deltaTime;
            scaleTimer += Time.deltaTime;

            credibilityBar.fillAmount = Mathf.Lerp(currentFill, targetFill, timer / fillBarDuration);

            ScaleCredibilityIcon(ref isIncreasingIconSize, ref scaleTimer);

            float currentPerc = credibilityBar.fillAmount * 100f;

            credibilityPanel.alpha = Mathf.Lerp(0f, 1f, timer / fadingDuration);

            if (credibilityPerc >= 100f)
                credibilityBarOutline.fillAmount = Mathf.SmoothStep(0f, 1f, timer / fillBarDuration);

            yield return new WaitForEndOfFrame();
        }

        timer = 0f;

        while (timer < idleBarDuration)
        {
            timer += Time.deltaTime;
            scaleTimer += Time.deltaTime;
            outlineFlashTimer += Time.deltaTime;

            ScaleCredibilityIcon(ref isIncreasingIconSize, ref scaleTimer);
            
            if (outlineFlashTimer >= outlineFlashIntervals)
            {
                outlineFlashTimer = 0f;
                credibilityBarOutline.gameObject.SetActive(!credibilityBarOutline.gameObject.activeInHierarchy);
            }

            yield return new WaitForEndOfFrame();
        }

        timer = 0f;

        float previousIconScale = credibilityIcon.transform.localScale.x;

        while (timer < fadingDuration)
        {
            timer += Time.deltaTime;

            float newIconScale = Mathf.SmoothStep(previousIconScale, 1f, timer / fadingDuration);
            
            credibilityIcon.transform.localScale = new Vector3(newIconScale, newIconScale, newIconScale);
            credibilityPanel.alpha = Mathf.Lerp(1f, 0f, timer / fadingDuration);

            yield return new WaitForEndOfFrame();
        }

        credibilityPanel.gameObject.SetActive(false);

        fillingBarRoutine = null;
    }

    public void StartFillingBar(float credibilityPerc)
    {
        fillingBarRoutine = StartCoroutine(FillBar(credibilityPerc));
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
        credibilityBarOutline.gameObject.SetActive(true);
        credibilityBarOutline.fillAmount = 0f;
    }
}