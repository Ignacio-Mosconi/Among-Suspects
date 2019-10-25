using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] ScrollRect creditsScrollRect = default;
    [SerializeField, Range(0f, 300f)] float rollDuration = 10f;
    
    float elapsedTime;
    bool isRolling;

    void OnEnable()
    {
        creditsScrollRect.verticalNormalizedPosition = 1f;
        elapsedTime = 0f;
        isRolling = true;
    }

    public void OnBeginDrag(BaseEventData data)
    {
        isRolling = false;
    }

    public void OnEndDrag(BaseEventData data)
    {
        float interpolationValue = Mathf.InverseLerp(1f, 0f, creditsScrollRect.verticalNormalizedPosition);

        elapsedTime = rollDuration * interpolationValue;
        isRolling = true;
    }

    void Update()
    {
        if (creditsScrollRect.verticalNormalizedPosition > 0f && isRolling)
        {
            elapsedTime += Time.deltaTime;

            creditsScrollRect.verticalNormalizedPosition = Mathf.Lerp(1f, 0f, elapsedTime / rollDuration);
        }
    }
}