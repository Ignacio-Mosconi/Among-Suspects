using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] [Range(1f, 3f)] float fadeDuration = 2f;

    Image blackImage;
    Coroutine fadingRoutine;

    void Awake()
    {
        blackImage = GetComponentInChildren<Image>(includeInactive: true);
    }

    IEnumerator Fade(float initialAlpha, float targetAlpha)
    {
        float fadeTimer = 0f;
        Color newFaderColor = blackImage.color;

        blackImage.gameObject.SetActive(true);

        while (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            newFaderColor.a = Mathf.Lerp(initialAlpha, targetAlpha, fadeTimer / fadeDuration);
            blackImage.color = newFaderColor;

            yield return new WaitForEndOfFrame();
        }

        blackImage.gameObject.SetActive(false);

        fadingRoutine = null;
    }

    public void FadeInScene()
    {
        if (fadingRoutine != null)
            StopCoroutine(fadingRoutine);
        StartCoroutine(Fade(1f, 0f));
    }

    public void FadeOutScene()
    {
        if (fadingRoutine != null)
            StopCoroutine(fadingRoutine);
        StartCoroutine(Fade(0f, 1f));
    }

    #region Properties

    public float FadeDuration
    {
        get { return fadeDuration; }
    }

    #endregion
}