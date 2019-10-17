using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class DialogueOptionsScreen : MonoBehaviour
{
    [SerializeField] VerticalLayoutGroup optionsLayout = default;

    Button[] optionsButtons;
    Dictionary<Language, DialogueOption[]> currentDialogueOptionsByLanguage;
    bool isSelectingOption = false;
    int[] regularOptionsLayoutPadding = { 0, 0 };
    float regularOptionsLayoutSpacing = 0f;

    void Awake()
    {
        optionsButtons = optionsLayout.GetComponentsInChildren<Button>(includeInactive: true);
    }

    void Start()
    {
        for (int i = 0; i < optionsButtons.Length; i++)
        {
            int optionIndex = i;
            optionsButtons[i].onClick.AddListener(() => SelectOption(optionIndex));
            GameManager.Instance.AddCursorPointerEvents(optionsButtons[i]);
        }

        regularOptionsLayoutPadding[0] = optionsLayout.padding.top;
        regularOptionsLayoutPadding[1] = optionsLayout.padding.bottom;
        regularOptionsLayoutSpacing = optionsLayout.spacing;
    }

    void SelectOption(int option)
    {
        isSelectingOption = false;

        foreach (Button optionButton in optionsButtons)
            optionButton.gameObject.SetActive(false);

        optionsLayout.padding.top = regularOptionsLayoutPadding[0];
        optionsLayout.padding.bottom = regularOptionsLayoutPadding[1];
        optionsLayout.spacing = regularOptionsLayoutSpacing;

        GameManager.Instance.SetCursorEnable(enable: false);

        DialogueManager.Instance.ResumeInteractiveDialogue(option);
    }

    public void ShowOptionsScreen(Dictionary<Language, DialogueOption[]> dialogueOptionsByLanguage)
    {
        isSelectingOption = true;
        currentDialogueOptionsByLanguage = dialogueOptionsByLanguage;
        
        ChangeOptionsLanguage();

        int optionsLayoutPaddingMult = optionsButtons.Length - dialogueOptionsByLanguage[GameManager.Instance.CurrentLanguage].Length;
        int addtionalPadding = (int)optionsButtons[0].GetComponent<Image>().rectTransform.sizeDelta.y / optionsButtons.Length;

        optionsLayout.padding.top = regularOptionsLayoutPadding[0] + (addtionalPadding * optionsLayoutPaddingMult);
        optionsLayout.padding.bottom = regularOptionsLayoutPadding[1] + (addtionalPadding * optionsLayoutPaddingMult);
        optionsLayout.spacing = regularOptionsLayoutSpacing + (addtionalPadding * optionsLayoutPaddingMult);

        GameManager.Instance.SetCursorEnable(enable: true);
    }

    public void ChangeOptionsLanguage()
    {
        DialogueOption[] dialogueOptions = currentDialogueOptionsByLanguage[GameManager.Instance.CurrentLanguage];

        for (int i = 0; i < dialogueOptions.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(true);

            TextMeshProUGUI[] optionTexts = optionsButtons[i].gameObject.GetComponentsInChildren<TextMeshProUGUI>();

            optionTexts[0].text = dialogueOptions[i].option;
            optionTexts[1].text = dialogueOptions[i].description;
        }
    }

    #region Properties

    public bool IsSelectingOption
    {
        get { return isSelectingOption; }
    }

    #endregion 
}