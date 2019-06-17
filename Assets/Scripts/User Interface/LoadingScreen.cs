using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Image loadingBarForeground = default;
    [SerializeField] TextMeshProUGUI loadingText = default;

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

        if (percentage == 100f)
            percentageText = percentage.ToString();
        else
            percentageText = (percentage > 10f) ? " " + percentage.ToString() : "  " + percentage.ToString();

        loadingBarForeground.fillAmount = progressValue;
        loadingText.text = "Loading: " + percentageText + "%"; 
    }
}