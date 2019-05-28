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

    Button[] cluesButtons;
    PlayerController playerController;

    void Awake()
    {
        cluesButtons = cluesButtonsPanel.GetComponentsInChildren<Button>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnEnable()
    {
        bool firstClueFound = false;

        for (int i = 0; i < cluesButtons.Length; i++)
        {
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);
            TextMeshProUGUI buttonText = cluesButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            
            cluesButtons[i].interactable = playerController.HasClue(ref clueInfo);

            if (cluesButtons[i].IsInteractable())
            {
                buttonText.text = clueInfo.clueName;

                if (!firstClueFound)
                {
                    clueTitleText.text = clueInfo.clueName;
                    clueDescriptionText.text = clueInfo.description;
                    clueImage.sprite = clueInfo.clueSprite;
                    firstClueFound = true;
                }
            }
            else
                buttonText.text = "???";
        }

        cluesDescriptionArea.SetActive(firstClueFound);
    }

    public void SelectClue(int clueIndex)
    {
        ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(clueIndex);
        
        clueTitleText.text = (clueInfo) ? clueInfo.clueName : "???" ;
        clueDescriptionText.text = (clueInfo) ? clueInfo.description : "";
        clueImage.sprite = clueInfo.clueSprite;
    }
}