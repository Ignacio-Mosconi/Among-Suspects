using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] GameObject debateArea;
    [SerializeField] GameObject speakerTextPanel;
    [SerializeField] GameObject argumentTextPanel;
    [SerializeField] GameObject debateOptionsPanel;
    [SerializeField] GameObject clueOptionsPanel;
    [SerializeField] VerticalLayoutGroup clueOptionsLayout;
    [SerializeField] Button clueOptionsBackButton;
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] TextMeshProUGUI argumentText;
    [SerializeField] [Range(30f, 60f)] float cameraRotSpeed = 60f;
    [SerializeField] [Range(1f, 2f)] float argumentPanelExpandScale = 2f;
    [SerializeField] [Range(0.5f, 1.5f)] float argumentPanelScaleDur = 1f;
    
    DebateInitializer debateInitializer;
    DebateInfo currentDebateInfo;
    Argument currentArgument;
    DebateDialogue[] currentLines;
    Coroutine focusingRoutine;
    Coroutine expandindArgumentRoutine;
    Quaternion currentCamTargetRot;
    CharacterName previousSpeaker = CharacterName.None;
    List<ClueInfo> caseClues;
    int lineIndex = 0;
    int[] regularCluesLayoutPadding = { 0, 0 };
    float regularCluesLayoutSpacing = 0f;

    void Start()
    {
        regularCluesLayoutPadding[0] = clueOptionsLayout.padding.top;
        regularCluesLayoutPadding[1] = clueOptionsLayout.padding.bottom;
        regularCluesLayoutSpacing = clueOptionsLayout.spacing;

        enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Continue"))
        {
            if (focusingRoutine != null)
            {
                FinishFocus();
                return;
            }

            if (expandindArgumentRoutine != null)
            {
                FinishArgumentExpansion();
                return;
            }

            ResetDebatePanelsStatuses();

            lineIndex++;
            if (lineIndex < currentLines.Length) 
                Argue(currentLines[lineIndex].speakerName, currentLines[lineIndex].argument, currentLines[lineIndex].characterEmotion);
            else
            {
                lineIndex = 0;
                ShowDebateOptions();
            }
        }
    }

    void SetDebateAreaAvailability(bool enableDebateArea)
    {
        debateArea.SetActive(enableDebateArea);
        enabled = enableDebateArea;
    }

    void ResetDebatePanelsStatuses()
    {
        speakerTextPanel.SetActive(false);
        argumentTextPanel.SetActive(false);

        argumentTextPanel.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void ShowDebateOptions()
    {
        enabled = false;
        debateOptionsPanel.SetActive(true);
    }

    void Argue(CharacterName speaker, string argument, CharacterEmotion speakerEmotion)
    {
        SpriteRenderer characterRenderer = debateInitializer.GetCharacterSpriteRenderer(speaker);

        if (speaker != previousSpeaker)
        {
            Vector3 charPosition = characterRenderer.transform.position;

            focusingRoutine = StartCoroutine(FocusOnCharacter(charPosition));     
            speakerText.text = speaker.ToString();
            previousSpeaker = speaker;
        }
        else
            SayArgument();

        argumentText.text = argument;
        characterRenderer.sprite = CharacterManager.Instance.GetCharacter(speaker).GetSprite(speakerEmotion);
    }

    void SayArgument()
    {
        speakerTextPanel.SetActive(true);
        argumentTextPanel.SetActive(true);

        expandindArgumentRoutine = StartCoroutine(ExpandArgumentPanel());
    }

    void FinishFocus()
    {
        if (focusingRoutine != null)
        {
            StopCoroutine(focusingRoutine);
            debateInitializer.DebateCamera.transform.rotation = currentCamTargetRot;
            SayArgument();
            focusingRoutine = null;
        }
    }

    void FinishArgumentExpansion()
    {
        if (expandindArgumentRoutine != null)
        {
            StopCoroutine(expandindArgumentRoutine);
            argumentTextPanel.transform.localScale = new Vector3(argumentPanelExpandScale, argumentPanelExpandScale, argumentPanelExpandScale);
            expandindArgumentRoutine = null;
        }    
    }

    IEnumerator FocusOnCharacter(Vector3 characterPosition)
    {
        Vector3 diff = characterPosition - debateInitializer.DebateCamera.transform.position;

        Vector3 fromDir = debateInitializer.DebateCamera.transform.forward;
        Vector3 targetDir = new Vector3(diff.x, debateInitializer.DebateCamera.transform.forward.y, diff.z);
        Quaternion fromRot = debateInitializer.DebateCamera.transform.rotation;
        
        currentCamTargetRot = Quaternion.FromToRotation(fromDir, targetDir);

        float timer = 0f;
        float angleBetweenRots = Quaternion.Angle(fromRot, currentCamTargetRot);
        float rotDuration = angleBetweenRots / cameraRotSpeed;

        while (debateInitializer.DebateCamera.transform.rotation != currentCamTargetRot)
        {
            timer += Time.deltaTime;
            debateInitializer.DebateCamera.transform.rotation = Quaternion.Slerp(fromRot, currentCamTargetRot, timer / rotDuration);

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

    public void AgreeToComment()
    {
        debateOptionsPanel.SetActive(false);
        Debug.Log("I Agree.");

        if (currentArgument.correctReaction == DebateReaction.Agree)
            Debug.Log("You selected the correct option.");
        else
            Debug.Log("You didn't select the correct option.");
    }

    public void DisagreeToComment()
    {
        debateOptionsPanel.SetActive(false);
        clueOptionsPanel.gameObject.SetActive(true);
    }

    public void AccuseWithEvidence(int optionIndex)
    {
        clueOptionsPanel.SetActive(false);
        
        Debug.Log("That's wrong!");
        
        if (currentArgument.correctReaction == DebateReaction.Disagree)
        {
            Debug.Log("You selected the correct option.");
            
            if (caseClues[optionIndex] == currentArgument.correctEvidence)
                Debug.Log("That's the correct evidence.");
            else
                Debug.Log("That's not the correct evidence.");
        }
        else
            Debug.Log("You didn't select the correct option.");
    }

    public void ReturnToDebateOptions()
    {
        debateOptionsPanel.SetActive(true);
        clueOptionsPanel.SetActive(false);
    }

    public void EnableDebateArea(DebateInitializer initializer, List<ClueInfo> playerClues)
    {
        debateInitializer = initializer;
        initializer.DebateCamera.gameObject.SetActive(true);

        currentDebateInfo = initializer.DebateInfo;
        currentArgument = currentDebateInfo.arguments[0];
        currentLines = currentArgument.debateDialogue;

        Button[] clueOptions = clueOptionsLayout.GetComponentsInChildren<Button>(includeInactive: true);
        
        caseClues = playerClues;

        int i = 0;
        int normalButtonHeight = (int)clueOptions[0].GetComponent<RectTransform>().rect.height;

        for (i = 0; i < caseClues.Count; i++)
        {
            clueOptions[i].gameObject.SetActive(true);

            TextMeshProUGUI clueText = clueOptions[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();
            clueText.text = caseClues[i].clueName;
        }

        int cluesLayoutPaddingMult = clueOptions.Length - i;
        int addtionalPadding = (i > 1) ? normalButtonHeight / 3 * cluesLayoutPaddingMult : normalButtonHeight / 2 * cluesLayoutPaddingMult;

        clueOptionsLayout.padding.top = regularCluesLayoutPadding[0] + addtionalPadding;
        clueOptionsLayout.padding.bottom = regularCluesLayoutPadding[1] + addtionalPadding;
        clueOptionsLayout.spacing = regularCluesLayoutSpacing + addtionalPadding;

        SetDebateAreaAvailability(enableDebateArea: true);

        Argue(currentLines[0].speakerName, currentLines[0].argument, currentLines[0].characterEmotion);
    }  
}