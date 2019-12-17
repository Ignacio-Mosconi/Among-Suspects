using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CredibilityBarController : MonoBehaviour
{
    [SerializeField] CanvasGroup credibilityPanel = default;
    [SerializeField] Image credibilityBar = default;
    [SerializeField] Image secondaryCredibilityBar = default;
    [SerializeField] Image credibilityBarOutline = default;
    [SerializeField] Image credibilityIcon = default;
    [SerializeField] [Range(1f, 3f)] float fillBarDuration = 1.5f;
    [SerializeField] [Range(0.5f, 1f)] float secondaryFillBarDuration = 0.75f;
    [SerializeField] [Range(1f, 2f)] float idleBarDuration = 1.5f;
    [SerializeField] [Range(0.5f, 1f)] float fadingDuration = 0.75f;
    [SerializeField] [Range(0.25f, 0.5f)] float iconScaleDuration = 0.4f;
    [SerializeField] [Range(0.2f, 0.75f)] float outlineFlashIntervals = 0.3f;
    [SerializeField] [Range(1f, 1.3f)] float maxIconScale = 1.2f;
    [SerializeField] [Range(0.7f, 1f)] float minIconScale = 0.8f;
    [SerializeField] Sprite[] credibilitySprites = default;
    [SerializeField] Color[] secondaryBarColors = default;

    Coroutine fillingBarRoutine;
    float scaleTimer = 0f;
    bool isIncreasingIconSize = true;

    void ScaleCredibilityIcon()
    {
        float newIconScale = (isIncreasingIconSize) ? Mathf.SmoothStep(1f, maxIconScale, scaleTimer / iconScaleDuration) :
                                            Mathf.SmoothStep(1f, minIconScale, scaleTimer / iconScaleDuration);

        credibilityIcon.transform.localScale = new Vector3(newIconScale, newIconScale, newIconScale);

        if (scaleTimer >= iconScaleDuration)
        {
            scaleTimer = 0f;
            isIncreasingIconSize = !isIncreasingIconSize;
        }
    }

    IEnumerator FadeBarPanelIn()
    {
        float timer = 0;
        
        while (timer < fadingDuration)
        {
            timer += Time.deltaTime;

            credibilityPanel.alpha = Mathf.Lerp(0f, 1f, timer / fadingDuration);

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeBarPanelOut(float lastIconScale)
    {
        float timer = 0;

        while (timer < fadingDuration)
        {
            timer += Time.deltaTime;

            float newIconScale = Mathf.SmoothStep(lastIconScale, 1f, timer / fadingDuration);

            credibilityIcon.transform.localScale = new Vector3(newIconScale, newIconScale, newIconScale);
            credibilityPanel.alpha = Mathf.Lerp(1f, 0f, timer / fadingDuration);

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FillMainBar(float currentFill, float targetFill, float credibilityPerc)
    {
        float timer = 0;

        while (timer < fillBarDuration)
        {
            timer += Time.deltaTime;
            scaleTimer += Time.deltaTime;

            credibilityBar.fillAmount = Mathf.Lerp(currentFill, targetFill, timer / fillBarDuration);

            ScaleCredibilityIcon();

            if (credibilityPerc >= 100f)
                credibilityBarOutline.fillAmount = Mathf.SmoothStep(0f, 1f, timer / fillBarDuration);

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FillSecondaryBar(float currentFill, float targetFill)
    {
        float timer = 0;

        while (timer < secondaryFillBarDuration)
        {
            timer += Time.deltaTime;
            scaleTimer += Time.deltaTime;

            secondaryCredibilityBar.fillAmount = Mathf.Lerp(currentFill, targetFill, timer / secondaryFillBarDuration);
            ScaleCredibilityIcon();

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FlashBarOnIdle()
    {
        float timer = 0f;
        float outlineFlashTimer = 0f;

        while (timer < idleBarDuration)
        {
            timer += Time.deltaTime;
            scaleTimer += Time.deltaTime;
            outlineFlashTimer += Time.deltaTime;

            ScaleCredibilityIcon();

            if (outlineFlashTimer >= outlineFlashIntervals)
            {
                outlineFlashTimer = 0f;
                credibilityBarOutline.gameObject.SetActive(!credibilityBarOutline.gameObject.activeInHierarchy);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FillBar(float credibilityPerc)
    {
        string eventToPlayName;
        float currentFill = credibilityBar.fillAmount;
        float targetFill = credibilityPerc / 100f;
        
        isIncreasingIconSize = true;

        if (targetFill > currentFill)
        {
            credibilityIcon.sprite = credibilitySprites[0];
            eventToPlayName = "Debate_Correct";
            secondaryCredibilityBar.color = secondaryBarColors[0];
        }
        else
        {
            credibilityIcon.sprite = credibilitySprites[1];
            eventToPlayName = "Debate_Incorrect";
            secondaryCredibilityBar.color = secondaryBarColors[1];
        }

        AudioManager.Instance.PostEvent(eventToPlayName);

        credibilityBarOutline.fillAmount = 0f;

        credibilityPanel.gameObject.SetActive(true);
        credibilityBarOutline.gameObject.SetActive(true);

        yield return StartCoroutine(FadeBarPanelIn());

        secondaryCredibilityBar.gameObject.SetActive(true);

        if (targetFill > currentFill)
            yield return StartCoroutine(FillSecondaryBar(currentFill, targetFill));

        yield return StartCoroutine(FillMainBar(currentFill, targetFill, credibilityPerc));
        
        yield return StartCoroutine(FlashBarOnIdle());

        if (targetFill < currentFill)
            yield return StartCoroutine(FillSecondaryBar(currentFill, targetFill));

        secondaryCredibilityBar.gameObject.SetActive(false);

        float previousIconScale = credibilityIcon.transform.localScale.x;

        yield return StartCoroutine(FadeBarPanelOut(previousIconScale));

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
            StopAllCoroutines();
            fillingBarRoutine = null;
        }
    }

    public bool IsFillingBar()
    {
        return (fillingBarRoutine != null);
    }

    public void ResetCredibilityBar(float credibilityPerc)
    {
        credibilityBar.fillAmount = secondaryCredibilityBar.fillAmount = credibilityPerc / 100f;
        credibilityIcon.sprite = credibilitySprites[1];
        credibilityIcon.transform.localScale = new Vector3(1f, 1f, 1f);
        credibilityBarOutline.gameObject.SetActive(true);
        credibilityBarOutline.fillAmount = 0f;
    }
}