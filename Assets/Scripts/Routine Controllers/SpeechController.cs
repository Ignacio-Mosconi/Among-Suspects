using System.Collections;
using UnityEngine;
using TMPro;

public class SpeechController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI speechText = default;
    
    int targetSpeechCharAmount;
    
    Coroutine speakingRoutine;

    IEnumerator Speak()
    {
        while (speechText.maxVisibleCharacters != targetSpeechCharAmount)
        {
            speechText.maxVisibleCharacters++;
            yield return new WaitForSeconds(GameManager.Instance.CharactersShowIntervals);
        }

        speakingRoutine = null;
    }

    public void StartSpeaking(string speech)
    {
        speechText.maxVisibleCharacters = 0;
        speechText.text = speech;
        targetSpeechCharAmount = speech.Length;

        speakingRoutine = StartCoroutine(Speak());
    }

    public void StopSpeaking()
    {
        if (speakingRoutine != null)
        {
            StopCoroutine(speakingRoutine);
            speechText.maxVisibleCharacters = targetSpeechCharAmount;
            speakingRoutine = null;
        }
    }

    public bool IsSpeaking()
    {
        return (speakingRoutine != null);
    }
}