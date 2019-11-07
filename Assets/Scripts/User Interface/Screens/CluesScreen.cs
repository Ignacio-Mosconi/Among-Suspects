using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class CluesScreen : MonoBehaviour
{
    [SerializeField] GameObject cluesButtonsPanel = default;
    [SerializeField] UIPrompt clueDescriptionPrompt = default;
    [SerializeField] TextMeshProUGUI clueTitleText = default;
    [SerializeField] TextMeshProUGUI clueDescriptionText = default;
    [SerializeField] Image clueImage = default;

    List<Button> cluesButtons = new List<Button>();
    RectTransform cluesButtonsPanelsRectTrans;
    GameObject clueButtonPrefab;
    Button lastButtonSelected;
    Language previousLanguage;
    float addedClueButtonsPanelSize;

    UnityEvent onClueDeselected = new UnityEvent();

    const string ClueButtonPrefabPath = "Menu Elements/Clue Button";

    void Awake()
    {
        VerticalLayoutGroup cluesButtonsPanelVerLay = cluesButtonsPanel.GetComponent<VerticalLayoutGroup>();

        previousLanguage = GameManager.Instance.CurrentLanguage;
        
        cluesButtonsPanelsRectTrans = cluesButtonsPanel.GetComponent<RectTransform>();
        
        clueButtonPrefab = Resources.Load(ClueButtonPrefabPath) as GameObject;
        cluesButtonsPanelsRectTrans.sizeDelta = new Vector2(cluesButtonsPanelsRectTrans.sizeDelta.x,
                                                            cluesButtonsPanelVerLay.padding.top + cluesButtonsPanelVerLay.padding.bottom);

        int cluesAmount = ChapterManager.Instance.CluesAmount;
        float clueButtonHeight = clueButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        
        addedClueButtonsPanelSize = clueButtonHeight + cluesButtonsPanelVerLay.spacing;

        for (int i = 0; i < cluesAmount; i++)
        {
            GameObject clueButtonObject = Instantiate(clueButtonPrefab, cluesButtonsPanel.transform);
            Button clueButton = clueButtonObject.GetComponent<Button>();
            TextMeshProUGUI buttonText = clueButton.GetComponentInChildren<TextMeshProUGUI>();
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);

            buttonText.text = clueInfo.clueName;

            clueButton.onClick.AddListener(() => StartCoroutine(SelectClue(clueInfo, clueButton)));
            clueButton.gameObject.SetActive(false);
            cluesButtons.Add(clueButton);
        }

        clueDescriptionPrompt.SetUp();
        clueDescriptionPrompt.Deactivate();
    }

    void OnEnable()
    {
        if (GameManager.Instance.CurrentLanguage != previousLanguage)
            ReloadCluesInCurrentLanguage();

        int foundClueIndex = 0;

        for (int i = 0; i < cluesButtons.Count; i++)
        {
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);

            if (!cluesButtons[i].gameObject.activeSelf && CharacterManager.Instance.PlayerController.HasClue(ref clueInfo))
            {
                cluesButtons[i].gameObject.SetActive(true);
                cluesButtonsPanelsRectTrans.sizeDelta = new Vector2(cluesButtonsPanelsRectTrans.sizeDelta.x, 
                                                                    cluesButtonsPanelsRectTrans.sizeDelta.y + addedClueButtonsPanelSize);
            }

            if (cluesButtons[i].gameObject.activeSelf)
            {
                cluesButtons[i].transform.SetSiblingIndex(foundClueIndex);
                foundClueIndex++;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            GameObject currentlySelectedObject = EventSystem.current.currentSelectedGameObject;

            if ((!currentlySelectedObject || (lastButtonSelected && currentlySelectedObject != lastButtonSelected.gameObject)) &&
                clueDescriptionPrompt.gameObject.activeInHierarchy && !clueDescriptionPrompt.IsHiding)
            {
                lastButtonSelected = null;
                clueDescriptionPrompt.Hide();
                onClueDeselected.Invoke();
            }
        }
    }

    void ReloadCluesInCurrentLanguage()
    {
        previousLanguage = GameManager.Instance.CurrentLanguage;

        for (int i = 0; i < cluesButtons.Count; i++)
        {
            Button clueButton = cluesButtons[i];
            TextMeshProUGUI buttonText = clueButton.GetComponentInChildren<TextMeshProUGUI>();
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);

            buttonText.text = clueInfo.clueName;

            clueButton.onClick.RemoveAllListeners();
            clueButton.onClick.AddListener(() => SelectClue(clueInfo, clueButton));
        }
    }

    IEnumerator SelectClue(ClueInfo clueInfo, Button clueButton)
    {
        if (lastButtonSelected == clueButton && clueDescriptionPrompt.gameObject.activeInHierarchy)
            yield break;

        lastButtonSelected = clueButton;

        clueDescriptionPrompt.Hide();

        yield return new WaitForSecondsRealtime(clueDescriptionPrompt.HideAnimationDuration);

        clueTitleText.text = clueInfo.clueName;
        clueDescriptionText.text = clueInfo.description;
        clueImage.sprite = clueInfo.clueSprite;

        clueDescriptionPrompt.Show();
    }

    #region Properties

    public List<Button> CluesButtons
    {
        get { return cluesButtons; }
    }

    public UnityEvent OnClueDeselected
    {
        get { return onClueDeselected; }
    }

    #endregion
}