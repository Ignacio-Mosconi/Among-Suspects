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
    RectTransform cluesButtonsPanelsRectTrans;
    GameObject clueButtonPrefab;
    float addedClueButtonsPanelSize;

    const string ClueButtonPrefabPath = "Menu Elements/Clue Button";

    void Awake()
    {
        VerticalLayoutGroup cluesButtonsPanelVerLay = cluesButtonsPanel.GetComponent<VerticalLayoutGroup>();
        
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
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);

            clueButton.onClick.AddListener(() => SelectClue(clueInfo));
            clueButton.gameObject.SetActive(false);
            cluesButtons.Add(clueButton);
        }
    }

    void OnEnable()
    {
        int foundClueIndex = 0;

        for (int i = 0; i < cluesButtons.Count; i++)
        {
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);
            Image buttonImage = cluesButtons[i].GetComponent<Image>();
            TextMeshProUGUI buttonText = cluesButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (CharacterManager.Instance.PlayerController.HasClue(ref clueInfo) && !cluesButtons[i].gameObject.activeSelf)
            {
                cluesButtons[i].gameObject.SetActive(true);
                cluesButtonsPanelsRectTrans.sizeDelta = new Vector2(cluesButtonsPanelsRectTrans.sizeDelta.x, 
                                                                    cluesButtonsPanelsRectTrans.sizeDelta.y + addedClueButtonsPanelSize);
            }

            if (cluesButtons[i].gameObject.activeSelf)
            {
                buttonText.text = clueInfo.clueName;

                if (foundClueIndex == 0)
                    SelectClue(clueInfo);

                cluesButtons[i].transform.SetSiblingIndex(foundClueIndex);
                foundClueIndex++;
            }
        }

        cluesDescriptionArea.SetActive(foundClueIndex != 0);
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