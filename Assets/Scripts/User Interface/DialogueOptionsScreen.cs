using UnityEngine;
using UnityEngine.UI;
using TMPro;

class DialogueOptionsScreen : MonoBehaviour
{
    [SerializeField] VerticalLayoutGroup optionsLayout = default;

    Button[] optionsButtons;
    int[] regularOptionsLayoutPadding = { 0, 0 };
    float regularOptionsLayoutSpacing = 0f;

    void Awake()
    {
        optionsButtons = optionsLayout.GetComponentsInChildren<Button>(includeInactive: true);

        for (int i = 0; i < optionsButtons.Length; i++)
        {
            int optionIndex = i;
            optionsButtons[i].onClick.AddListener(() => SelectOption(optionIndex));
        }

        regularOptionsLayoutPadding[0] = optionsLayout.padding.top;
        regularOptionsLayoutPadding[1] = optionsLayout.padding.bottom;
        regularOptionsLayoutSpacing = optionsLayout.spacing;
    }

    void SelectOption(int option)
    {
        GameManager.Instance.SetCursorEnable(enable: false);

        foreach (Button optionButton in optionsButtons)
            optionButton.gameObject.SetActive(false);

        optionsLayout.padding.top = regularOptionsLayoutPadding[0];
        optionsLayout.padding.bottom = regularOptionsLayoutPadding[1];
        optionsLayout.spacing = regularOptionsLayoutSpacing;

        DialogueManager.Instance.ResumeInteractiveDialogue(option);
    }

    public void ShowOptionsScreen(InteractiveDialogue[] interactiveConversation)
    {
        GameManager.Instance.SetCursorEnable(enable: true);

        int i = 0;

        for (i = 0; i < interactiveConversation.Length; i++)
        {
            optionsButtons[i].gameObject.SetActive(true);

            TextMeshProUGUI[] optionTexts = optionsButtons[i].gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            
            optionTexts[0].text = interactiveConversation[i].playerOption.option;
            optionTexts[1].text = interactiveConversation[i].playerOption.description;
        }

        int optionsLayoutPaddingMult = optionsButtons.Length - i;
        int addtionalPadding = (int)optionsButtons[0].GetComponent<Image>().rectTransform.sizeDelta.y / optionsButtons.Length;

        optionsLayout.padding.top = regularOptionsLayoutPadding[0] + (addtionalPadding * optionsLayoutPaddingMult);
        optionsLayout.padding.bottom = regularOptionsLayoutPadding[1] + (addtionalPadding * optionsLayoutPaddingMult);
        optionsLayout.spacing = regularOptionsLayoutSpacing + (addtionalPadding * optionsLayoutPaddingMult);
    }
}