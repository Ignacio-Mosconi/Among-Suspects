using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DebateManager : MonoBehaviour
{
    #region Singleton

    static DebateManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
    }

    public static DebateManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DebateManager>();
                if (!instance)
                    Debug.LogError("There is no 'Debate Manager' in the scene");
            }

            return instance;
        }
    }

    #endregion

    [SerializeField] DebateInfo debateInfo;
    [SerializeField] GameObject debateArea;
    [SerializeField] GameObject speakerTextPanel;
    [SerializeField] GameObject argumentTextPanel;
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] TextMeshProUGUI argumentText;
    [SerializeField] [Range(20f, 45f)] float cameraRotSpeed = 30f;
    [SerializeField] [Range(1f, 2f)] float argumentPanelExpandScale = 2f;
    [SerializeField] [Range(0.5f, 1.5f)] float argumentPanelScaleDur = 1f;
    
    DebateInitializer debateInitializer;
    DebateDialogue[] currentLines;
    Coroutine focusingRoutine;
    Coroutine expandindArgumentRoutine;

    void SetDebateAreaAvailability(bool enableDebateArea)
    {
        debateArea.SetActive(enableDebateArea);
    }

    void StartArgument(CharacterName speaker, string argument, CharacterEmotion speakerEmotion)
    {
        SpriteRenderer characterRenderer = debateInitializer.GetCharacterSpriteRenderer(speaker);
        Vector3 charPosition = characterRenderer.transform.position;

        focusingRoutine = StartCoroutine(FocusOnCharacter(charPosition));
        
        speakerText.text = speaker.ToString();
        argumentText.text = argument;
        characterRenderer.sprite = CharacterManager.Instance.GetCharacter(speaker).GetSprite(speakerEmotion);
    }

    void SayArgument()
    {
        speakerTextPanel.SetActive(true);
        argumentTextPanel.SetActive(true);

        expandindArgumentRoutine = StartCoroutine(ExpandArgumentPanel());
    }

    IEnumerator FocusOnCharacter(Vector3 characterPosition)
    {
        Vector3 diff = characterPosition - debateInitializer.DebateCamera.transform.position;

        Vector3 fromDir = debateInitializer.DebateCamera.transform.forward;
        Vector3 targetDir = new Vector3(diff.x, debateInitializer.DebateCamera.transform.forward.y, diff.z);
        Quaternion fromRot = debateInitializer.DebateCamera.transform.rotation;
        Quaternion targetRot = Quaternion.FromToRotation(fromDir, targetDir);

        float timer = 0f;
        float angleBetweenRots = Quaternion.Angle(fromRot, targetRot);
        float rotDuration = angleBetweenRots / cameraRotSpeed;

        while (debateInitializer.DebateCamera.transform.rotation != targetRot)
        {
            timer += Time.deltaTime;
            debateInitializer.DebateCamera.transform.rotation = Quaternion.Slerp(fromRot, targetRot, timer / rotDuration);

            yield return new WaitForEndOfFrame();
        }

        focusingRoutine = null;

        SayArgument();
    }

    IEnumerator ExpandArgumentPanel()
    {
        Vector3 initialScale = argumentTextPanel.transform.localScale;
        Vector3 targetScale = argumentTextPanel.transform.localScale * argumentPanelExpandScale;

        float timer = 0f;

        while (argumentTextPanel.transform.localScale != targetScale)
        {
            timer += Time.deltaTime;
            argumentTextPanel.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / argumentPanelScaleDur);

            yield return new WaitForEndOfFrame();
        }

        expandindArgumentRoutine = null;
    }

    public void EnableDebateArea(DebateInitializer initializer)
    {
        debateInitializer = initializer;
        initializer.DebateCamera.gameObject.SetActive(true);

        currentLines = debateInfo.arguments[0].debateDialogue;

        SetDebateAreaAvailability(enableDebateArea: true);

        StartArgument(currentLines[0].speakerName, currentLines[0].argument, currentLines[0].characterEmotion);
    }    
}