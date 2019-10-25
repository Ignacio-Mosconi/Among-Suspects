using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField] Image speakerImage = default;

    Coroutine slidingRoutine;
    Vector2 currentTargetPosition;

    IEnumerator Slide(Vector2 initialPosition, Vector2 targetPosition, float slideDuration)
    {
        float timer = 0f;

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;

            Vector2 currentPosition = Vector2.Lerp(initialPosition, targetPosition, timer / slideDuration);
            speakerImage.rectTransform.anchoredPosition = currentPosition;
            
            yield return new WaitForEndOfFrame();
        }

        slidingRoutine = null;
    }

    public void ShowImmediately()
    {
        speakerImage.gameObject.SetActive(true);
    }

    public void HideImmediately()
    {
        speakerImage.gameObject.SetActive(false);
    }

    public void ChangeSprite(Sprite sprite)
    {
        speakerImage.sprite = sprite;
    }

    public void SlideInFromLeft(float slideDuration)
    {
        Vector2 initialPosition = new Vector2(-Screen.width * 0.5f, speakerImage.rectTransform.anchoredPosition.y);
        currentTargetPosition = Vector2.zero;

        slidingRoutine = StartCoroutine(Slide(initialPosition, currentTargetPosition, slideDuration));
    }

    public void SlideInFromRight(float slideDuration)
    {
        Vector2 initialPosition = new Vector2(Screen.width * 0.5f, speakerImage.rectTransform.anchoredPosition.y);
        currentTargetPosition = Vector2.zero;

        if (slidingRoutine != null)
            StopSliding();
        slidingRoutine = StartCoroutine(Slide(initialPosition, currentTargetPosition, slideDuration));
    }

    public void StopSliding()
    {
        if (slidingRoutine != null)
        {
            StopCoroutine(slidingRoutine);
            speakerImage.rectTransform.anchoredPosition = currentTargetPosition;
            slidingRoutine = null;
        }
    }
}