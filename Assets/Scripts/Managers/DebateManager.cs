using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DebatePhase
{
    Dialoguing, Arguing, SolvingArgument, SolvingCase
}

[RequireComponent(typeof(SpeechController))]
[RequireComponent(typeof(ArgumentController))]
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

    [Header("UI Elements")]
    [Header("Areas")]
    [SerializeField] GameObject debateArea = default;
    [SerializeField] GameObject speakerArea = default;
    [SerializeField] GameObject argumentAndSpeechArea = default;
    [Header("Panels")]
    [SerializeField] GameObject argumentPanel = default;
    [SerializeField] GameObject speechPanel = default;
    [SerializeField] GameObject debateOptionsPanel = default;
    [SerializeField] GameObject clueOptionsPanel = default;
    [SerializeField] GameObject credibilityPanel = default;
    [Header("Layouts, Buttons, Images & Texts")]
    [SerializeField] VerticalLayoutGroup clueOptionsLayout = default;
    [SerializeField] TextMeshProUGUI speakerText = default;
    [SerializeField] TextMeshProUGUI argumentText = default;
    [SerializeField] TextMeshProUGUI speechText = default;
    [SerializeField] Image credibilityBar = default;
    [SerializeField] Image credibilityIcon = default;
    [Header("Debate Properties")]
    [SerializeField] [Range(30f, 60f)] float cameraRotSpeed = 60f;
    [SerializeField] [Range(0.5f, 1.5f)] float credibilityBarFillDur = 1f;
    [SerializeField] [Range(2f, 4f)] float credibilityBarIdleDur = 3f;
    [Header("Other UI Properties")]
    [SerializeField] Color credibilityBarColorPositive = Color.green;
    [SerializeField] Color credibilityBarColorNeutral = Color.yellow;
    [SerializeField] Color credibilityBarColorNegative = Color.red;
    [SerializeField] Sprite[] credibilitySprites = default;
    
    SpeechController speechController;
    ArgumentController argumentController;
    Camera debateCamera;
    DebateCharacterSprite[] debateCharactersSprites;
    DebateInfo currentDebateInfo;
    Argument currentArgument;
    Dialogue[] currentDialogueLines;
    DebateDialogue[] currentArgumentLines;
    Coroutine focusingRoutine;
    Coroutine fillingBarRoutine;
    Quaternion currentCamTargetRot;
    CharacterName previousSpeaker = CharacterName.None;
    List<ClueInfo> caseClues;
    DebatePhase currentPhase = DebatePhase.Dialoguing;
    bool caseWon = false;
    int lineIndex = 0;
    int argumentIndex = 0;
    int[] regularCluesLayoutPadding = { 0, 0 };
    float regularCluesLayoutSpacing = 0f;
    float regularClueButtonHeight = 0f;
    float credibilityPerc;
    float credibilityIncPerc;
    float credibilityDecPerc;
    bool isSelectingOption;

    const float MinCredibilityPercRequired = 70f;
    const float InitialCredibilityPerc = 50f;

    void Start()
    {
        DebateInitializer debateInitializer = FindObjectOfType<DebateInitializer>();

        speechController = GetComponent<SpeechController>();
        argumentController = GetComponent<ArgumentController>();

        argumentController.OnArgumentFinish.AddListener(ShowDebateOptions);
        
        debateCamera = debateInitializer.GetComponentInChildren<Camera>(includeInactive: true);
        debateCharactersSprites = debateInitializer.DebateCharactersSprites;

        regularCluesLayoutPadding[0] = clueOptionsLayout.padding.top;
        regularCluesLayoutPadding[1] = clueOptionsLayout.padding.bottom;
        regularCluesLayoutSpacing = clueOptionsLayout.spacing;

        Button clueOption = clueOptionsLayout.GetComponentInChildren<Button>(includeInactive: true);
        regularClueButtonHeight = clueOption.GetComponent<RectTransform>().rect.height;

        credibilityPerc = InitialCredibilityPerc;
        credibilityBar.fillAmount = credibilityPerc / 100f;
        credibilityBar.color = credibilityBarColorNeutral;

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

            if (argumentController.IsExpanding())
            {
                argumentController.StopExpanding(lastArgument: lineIndex == currentArgumentLines.Length - 1);
                return;
            }

            if (speechController.IsSpeaking())
            {
                speechController.StopSpeaking();
                return;
            }

            lineIndex++;

            switch (currentPhase)
            {
                case DebatePhase.Dialoguing:
                    
                    if (lineIndex < currentDialogueLines.Length)
                    {
                        Dialogue(currentDialogueLines[lineIndex].speakerName,
                                currentDialogueLines[lineIndex].speech,
                                currentDialogueLines[lineIndex].characterEmotion,
                                currentDialogueLines[lineIndex].playerThought);
                    }
                    else
                        StartArgumentPhase();
                    break;

                case DebatePhase.Arguing:
                    
                    ResetArgumentPanelScale();
                    if (lineIndex < currentArgumentLines.Length)
                    {
                        Argue(currentArgumentLines[lineIndex].speakerName,
                                currentArgumentLines[lineIndex].argument,
                                currentArgumentLines[lineIndex].speakerEmotion);
                    }
                    break;

                case DebatePhase.SolvingArgument:

                    if (lineIndex < currentDialogueLines.Length)
                    {
                        Dialogue(currentDialogueLines[lineIndex].speakerName,
                                currentDialogueLines[lineIndex].speech,
                                currentDialogueLines[lineIndex].characterEmotion,
                                currentDialogueLines[lineIndex].playerThought);
                    }
                    else
                    {
                        if (ShouldLoseCase())
                            EndCase(lose: true);
                        else
                        {
                            if (argumentIndex == currentDebateInfo.arguments.Length - 1)
                                EndCase();
                            else
                                StartNextArgument();
                        }
                    }
                    break;

                case DebatePhase.SolvingCase:
                    
                    if (lineIndex < currentDialogueLines.Length)
                    {
                        Dialogue(currentDialogueLines[lineIndex].speakerName,
                                currentDialogueLines[lineIndex].speech,
                                currentDialogueLines[lineIndex].characterEmotion,
                                currentDialogueLines[lineIndex].playerThought);
                    }
                    else
                    {
                        ChapterManager.Instance.ShowDebateEndScreen(caseWon);              
                        SetDebateAreaAvailability(enableDebateArea: false);
                    }

                    break;
            }
        }
    }

    bool ShouldLoseCase()
    {
        bool shouldLose;
        int argumentsRemaining = currentDebateInfo.arguments.Length - argumentIndex - 1;
        float maxAchievableCredibility = credibilityPerc + argumentsRemaining * credibilityIncPerc;

        shouldLose = (maxAchievableCredibility < MinCredibilityPercRequired) ? true : false;
        
        return shouldLose;
    }

    void SetDebateAreaAvailability(bool enableDebateArea)
    {
        debateArea.SetActive(enableDebateArea);
        enabled = enableDebateArea;

        if (!enableDebateArea)
        {
            StopAllCoroutines();

            focusingRoutine = null;
            fillingBarRoutine = null;

            currentDebateInfo = null;
            currentDialogueLines = null;
            currentArgumentLines = null;

            currentPhase = DebatePhase.Dialoguing;
            
            credibilityPerc = InitialCredibilityPerc;
            credibilityBar.fillAmount = credibilityPerc / 100f;
            credibilityBar.color = credibilityBarColorNeutral;

            caseWon = false;
            lineIndex = 0;
            argumentIndex = 0;

            ResetArgumentPanelScale();
            ResetCredibilityPanel();
            ResetMainUIVisibility();
        }
    }

    void ResetArgumentPanelScale()
    {
        argumentPanel.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void ResetCredibilityPanel()
    {
        credibilityPanel.SetActive(false);
    }

    void ResetMainUIVisibility()
    {
        speakerArea.SetActive(false);
        argumentAndSpeechArea.SetActive(false);
    }

    void ShowDebateOptions()
    {
        lineIndex = 0;
        enabled = false;
        isSelectingOption = true;
        debateOptionsPanel.SetActive(true);
        GameManager.Instance.SetCursorAvailability(enable: true);
    }

    void StartArgumentPhase()
    {
        lineIndex = 0;
        speechPanel.SetActive(false);
        currentPhase = DebatePhase.Arguing;
        Argue(currentArgumentLines[0].speakerName, currentArgumentLines[0].argument, currentArgumentLines[0].speakerEmotion);
    }

    void StartNextArgument()
    {
        lineIndex = 0;
        argumentIndex++;

        currentArgument = currentDebateInfo.arguments[argumentIndex];
        currentArgumentLines = currentArgument.debateDialogue;
        currentDialogueLines = currentArgument.argumentIntroDialogue;
        
        currentPhase = DebatePhase.Dialoguing;

        Dialogue(currentDialogueLines[0].speakerName,
                currentDialogueLines[0].speech,
                currentDialogueLines[0].characterEmotion,
                currentDialogueLines[0].playerThought);
    }

    void EndCase(bool lose = false)
    {
        currentPhase = DebatePhase.SolvingCase;
        caseWon = !lose;
        lineIndex = 0;
        currentDialogueLines = (!lose) ? currentDebateInfo.winDebateDialogue : currentDebateInfo.loseDebateDialogue;

        Dialogue(currentDialogueLines[0].speakerName,
                currentDialogueLines[0].speech,
                currentDialogueLines[0].characterEmotion,
                currentDialogueLines[0].playerThought);
    }

    void Argue(CharacterName speaker, string argument, CharacterEmotion speakerEmotion)
    {
        SpriteRenderer characterRenderer = Array.Find(debateCharactersSprites, cs => cs.characterName == speaker).spriteRenderer;

        argumentText.text = argument;

        ICharacter character = CharacterManager.Instance.GetCharacter(speaker);
        characterRenderer.sprite = character.GetSprite(speakerEmotion);

        if (speaker != previousSpeaker)
        {
            Vector3 charPosition = characterRenderer.transform.position;

            ResetMainUIVisibility();

            focusingRoutine = StartCoroutine(FocusOnCharacter(charPosition));     
            speakerText.text = speaker.ToString();
            previousSpeaker = speaker;
        }
        else
            SayArgument();
    }

    void Dialogue(CharacterName speaker, string speech, CharacterEmotion speakerEmotion, bool playerThought)
    {
        SpriteRenderer characterRenderer = Array.Find(debateCharactersSprites, cs => cs.characterName == speaker).spriteRenderer;

        ICharacter character = CharacterManager.Instance.GetCharacter(speaker);
        characterRenderer.sprite = character.GetSprite(speakerEmotion);

        if (playerThought)
        {
            if (speakerText.color != GameManager.Instance.PlayerThinkingTextColor)
                speechText.color = GameManager.Instance.PlayerThinkingTextColor;
        }
        else
        {
            if (speechText.color != GameManager.Instance.NpcSpeakingTextColor)
                speechText.color = GameManager.Instance.NpcSpeakingTextColor;
        }

        if (speaker != previousSpeaker)
        {
            Vector3 charPosition = characterRenderer.transform.position;

            ResetMainUIVisibility();

            focusingRoutine = StartCoroutine(FocusOnCharacter(charPosition));
            speakerText.text = speaker.ToString();
            previousSpeaker = speaker;
        }
        else
            SayDialogue(speech);
    }

    void SayArgument()
    {
        speakerArea.SetActive(true);
        argumentAndSpeechArea.SetActive(true);
        argumentPanel.SetActive(true);

        argumentController.StartExpanding(lastArgument: lineIndex == currentArgumentLines.Length - 1);
    }

    void SayDialogue(string speech)
    {
        speakerArea.SetActive(true);
        argumentAndSpeechArea.SetActive(true);
        speechPanel.SetActive(true);

        speechController.StartSpeaking(speech);
    }

    void FinishFocus()
    {
        if (focusingRoutine != null)
        {
            StopCoroutine(focusingRoutine);
            debateCamera.transform.rotation = currentCamTargetRot;
            if (currentPhase == DebatePhase.Arguing)
                SayArgument();
            else
                SayDialogue(currentDialogueLines[lineIndex].speech);
            focusingRoutine = null;
        }
    }

    void StopFillingCredibilityBar()
    {
        if (fillingBarRoutine != null)
        {
            StopCoroutine(fillingBarRoutine);
            fillingBarRoutine = null;
        }
    }

    IEnumerator FocusOnCharacter(Vector3 characterPosition)
    {
        Vector3 diff = characterPosition - debateCamera.transform.position;

        Vector3 targetDir = new Vector3(diff.x, debateCamera.transform.forward.y, diff.z);
        Quaternion fromRot = debateCamera.transform.rotation;
        
        currentCamTargetRot = Quaternion.LookRotation(targetDir, debateArea.transform.up);

        float timer = 0f;
        float angleBetweenRots = Quaternion.Angle(fromRot, currentCamTargetRot);
        float rotDuration = angleBetweenRots / cameraRotSpeed;

        while (timer < rotDuration)
        {
            timer += Time.deltaTime;
            debateCamera.transform.rotation = Quaternion.Slerp(fromRot, currentCamTargetRot, timer / rotDuration);

            yield return new WaitForEndOfFrame();
        }

        focusingRoutine = null;

        if (currentPhase == DebatePhase.Arguing)
            SayArgument();
        else
            SayDialogue(currentDialogueLines[lineIndex].speech);
    }

    IEnumerator ChangeCredibilityBarFill()
    {
        float timer = 0f;
        float currentFill = credibilityBar.fillAmount;
        float targetFill = credibilityPerc / 100f;

        credibilityPanel.SetActive(true);
        Invoke("ResetCredibilityPanel", credibilityBarIdleDur);

        while (currentFill != targetFill)
        {
            timer += Time.deltaTime;
            credibilityBar.fillAmount = Mathf.Lerp(currentFill, targetFill, timer / credibilityBarFillDur);

            Color newColor = credibilityBar.color;

            if (credibilityBar.fillAmount * 100f >= MinCredibilityPercRequired && credibilityBar.color != credibilityBarColorPositive)
            {
                newColor = credibilityBarColorPositive;
                credibilityIcon.sprite = credibilitySprites[0];
            }
            
            if (credibilityBar.fillAmount * 100f < MinCredibilityPercRequired && !ShouldLoseCase() &&
                credibilityBar.color != credibilityBarColorNeutral)
            {
                newColor = credibilityBarColorNeutral;
                credibilityIcon.sprite = credibilitySprites[1];
            }
            
            if (ShouldLoseCase() && credibilityBar.color != credibilityBarColorNegative)
            {
                newColor = credibilityBarColorNegative;
                credibilityIcon.sprite = credibilitySprites[2];
            }

            if (newColor != credibilityBar.color)
            {
                newColor.a = credibilityBar.color.a;
                credibilityBar.color = newColor;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void TrustComment()
    {
        ResetArgumentPanelScale();
        ResetMainUIVisibility();  
        
        debateOptionsPanel.SetActive(false);
        argumentPanel.SetActive(false);
        GameManager.Instance.SetCursorAvailability(false);

        currentDialogueLines = currentArgument.trustDialogue;
        
        currentPhase = DebatePhase.SolvingArgument;

        if (currentArgument.correctReaction == DebateReaction.Agree)
            credibilityPerc += credibilityIncPerc;
        else
            credibilityPerc -= credibilityDecPerc;

        if (fillingBarRoutine != null)
            StopFillingCredibilityBar();
        fillingBarRoutine = StartCoroutine(ChangeCredibilityBarFill());

        isSelectingOption = false;
        enabled = true;

        Dialogue(currentDialogueLines[0].speakerName,
                currentDialogueLines[0].speech,
                currentDialogueLines[0].characterEmotion,
                currentDialogueLines[0].playerThought);
    }

    public void RefuteComment()
    {
        debateOptionsPanel.SetActive(false);
        speakerArea.SetActive(false);
        argumentAndSpeechArea.SetActive(false);
        clueOptionsPanel.gameObject.SetActive(true);
    }

    public void AccuseWithEvidence(int optionIndex)
    {
        ResetArgumentPanelScale();
        ResetMainUIVisibility();

        clueOptionsPanel.SetActive(false);
        argumentPanel.SetActive(false);
        GameManager.Instance.SetCursorAvailability(false);
        
        currentPhase = DebatePhase.SolvingArgument;

        if (currentArgument.correctReaction == DebateReaction.Disagree && 
            currentArgument.correctEvidence == caseClues[optionIndex])
        {
            currentDialogueLines = currentArgument.refuteCorrectDialogue;
            credibilityPerc += credibilityIncPerc;
        }
        else
        {
            currentDialogueLines = currentArgument.refuteIncorrectDialogue;
            credibilityPerc -= credibilityDecPerc;
        }

        if (fillingBarRoutine != null)
            StopFillingCredibilityBar();
        fillingBarRoutine = StartCoroutine(ChangeCredibilityBarFill());

        isSelectingOption = false;
        enabled = true;

        Dialogue(currentDialogueLines[0].speakerName,
                currentDialogueLines[0].speech,
                currentDialogueLines[0].characterEmotion,
                currentDialogueLines[0].playerThought);
    }

    public void ReturnToDebateOptions()
    {
        debateOptionsPanel.SetActive(true);
        speakerArea.SetActive(true);
        argumentAndSpeechArea.SetActive(true);
        clueOptionsPanel.SetActive(false);
    }

    public void InitializeDebate(DebateInfo debateInfo, List<ClueInfo> playerClues)
    {
        debateCamera.gameObject.SetActive(true);

        currentDebateInfo = debateInfo;
        currentArgument = currentDebateInfo.arguments[0];
        currentDialogueLines = currentArgument.argumentIntroDialogue;
        currentArgumentLines = currentArgument.debateDialogue;

        credibilityIncPerc = credibilityPerc / currentDebateInfo.arguments.Length;
        credibilityDecPerc = credibilityIncPerc * 2f;

        Button[] clueOptions = clueOptionsLayout.GetComponentsInChildren<Button>(includeInactive: true);
                
        caseClues = playerClues;

        int i = 0;

        for (i = 0; i < caseClues.Count; i++)
        {
            clueOptions[i].gameObject.SetActive(true);

            TextMeshProUGUI clueText = clueOptions[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();
            clueText.text = caseClues[i].clueName;
        }

        int paddMult = clueOptions.Length - i;
        float addPadding = (regularClueButtonHeight + regularCluesLayoutSpacing) * paddMult * 0.5f;

        clueOptionsLayout.padding.top = regularCluesLayoutPadding[0] + (int)addPadding;
        clueOptionsLayout.padding.bottom = regularCluesLayoutPadding[1] + (int)addPadding;

        SetDebateAreaAvailability(enableDebateArea: true);

        Dialogue(currentDialogueLines[0].speakerName, 
                    currentDialogueLines[0].speech, 
                    currentDialogueLines[0].characterEmotion, 
                    currentDialogueLines[0].playerThought);
    }

    public void SetUpdateEnable(bool enable)
    {
        if (currentDebateInfo)
        {
            debateArea.SetActive(enable);
            if (!isSelectingOption)
                enabled = enable;
        }
    }
}