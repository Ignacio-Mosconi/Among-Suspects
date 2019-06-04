using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CluesScreen : MonoBehaviour
{
    [SerializeField] GameObject cluesButtonsPanel = default;
    [SerializeField] GameObject cluesDescriptionArea = default;
    [SerializeField] TextMeshProUGUI clueTitleText = default;
    [SerializeField] TextMeshProUGUI clueDescriptionText = default;
    [SerializeField] Image clueImage = default;

    List<Button> cluesButtons = new List<Button>();
    GameObject clueButtonPrefab;

    const string ClueButtonPrefabPath = "Menu Elements/Clue Button";

    void Awake()
    {
        RectTransform cluesButtonsPanelsRectTrans = cluesButtonsPanel.GetComponent<RectTransform>();
        VerticalLayoutGroup cluesButtonsPanelVerLay = cluesButtonsPanel.GetComponent<VerticalLayoutGroup>();
        
        clueButtonPrefab = Resources.Load(ClueButtonPrefabPath) as GameObject;
        cluesButtonsPanelsRectTrans.sizeDelta = new Vector2(cluesButtonsPanelsRectTrans.sizeDelta.x,
                                                            cluesButtonsPanelVerLay.padding.top + cluesButtonsPanelVerLay.padding.bottom);

        int cluesAmount = ChapterManager.Instance.CluesAmount;
        float clueButtonHeight = clueButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float addedPanelSize = clueButtonHeight + cluesButtonsPanelVerLay.spacing;

        for (int i = 0; i < cluesAmount; i++)
        {
            cluesButtonsPanelsRectTrans.sizeDelta = new Vector2(cluesButtonsPanelsRectTrans.sizeDelta.x, 
                                                                cluesButtonsPanelsRectTrans.sizeDelta.y + addedPanelSize);

            GameObject clueButtonObject = Instantiate(clueButtonPrefab, cluesButtonsPanel.transform);
            Button clueButton = clueButtonObject.GetComponent<Button>();
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);

            clueButton.onClick.AddListener(() => SelectClue(clueInfo));
            cluesButtons.Add(clueButton);
        }
    }

    void OnEnable()
    {
        bool oneClueFound = false;

        for (int i = 0; i < cluesButtons.Count; i++)
        {
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);
            TextMeshProUGUI buttonText = cluesButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            
            cluesButtons[i].interactable = CharacterManager.Instance.PlayerController.HasClue(ref clueInfo);

            if (cluesButtons[i].IsInteractable())
            {
                buttonText.text = clueInfo.clueName;

                if (!oneClueFound)
                {
                    SelectClue(clueInfo);
                    oneClueFound = true;
                }
            }
            else
                buttonText.text = "???";
        }

        cluesDescriptionArea.SetActive(oneClueFound);
    }

    void SelectClue(ClueInfo clueInfo)
    {   
        clueTitleText.text = clueInfo.clueName;
        clueDescriptionText.text = clueInfo.description;
        clueImage.sprite = clueInfo.clueSprite;
    }

    #region Properties

    public List<Button> CluesButtons
    {
        get { return cluesButtons; }
    }

    #endregion
}