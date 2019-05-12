using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CluesScreen : MonoBehaviour
{
    [SerializeField] GameObject cluesPanel;
    [SerializeField] TextMeshProUGUI clueTitleText;
    [SerializeField] TextMeshProUGUI clueDescriptionText;

    Button[] cluesButtons;
    PlayerController playerController;

    void Awake()
    {
        cluesButtons = cluesPanel.GetComponentsInChildren<Button>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnEnable()
    {
        bool isFirstClue = true;

        for (int i = 0; i < cluesButtons.Length; i++)
        {
            ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(i);
            TextMeshProUGUI buttonText = cluesButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            
            cluesButtons[i].interactable = playerController.HasClue(ref clueInfo);

            if (cluesButtons[i].IsInteractable())
            {
                buttonText.text = clueInfo.clueName;

                if (isFirstClue)
                {
                    clueTitleText.text = clueInfo.clueName;
                    clueDescriptionText.text = clueInfo.description;
                    isFirstClue = false;
                }
            }
            else
                buttonText.text = "???";
        }
    }

    public void SelectClue(int clueIndex)
    {
        ClueInfo clueInfo = ChapterManager.Instance.GetChapterClueInfo(clueIndex);
        
        clueTitleText.text = (clueInfo) ? clueInfo.clueName : "???" ;
        clueDescriptionText.text = (clueInfo) ? clueInfo.description : "";
    }
}