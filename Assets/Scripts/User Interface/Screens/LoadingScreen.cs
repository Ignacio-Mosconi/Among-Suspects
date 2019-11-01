using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Image loadingBarForeground = default;
    [SerializeField] TextMeshProUGUI loadingText = default;
    [SerializeField] TextMeshProUGUI loadingPercentageText = default;
    [SerializeField] string[] loadingTexts = new string[(int)Language.Count];

    public void Show()
    {
        ChangeLoadPercentage(0f);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ChangeLoadPercentage(float progressValue)
    {
        string percentageText;
        int percentage = (int)(progressValue * 100f);
        Language language = GameManager.Instance.CurrentLanguage;

        percentageText = percentage.ToString();

        loadingBarForeground.fillAmount = progressValue;
        loadingText.text = loadingTexts[(int)language] + ": ";
        loadingPercentageText.text = percentageText + "%";
    }
}