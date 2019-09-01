using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArgumentRecordUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI argumentNumberText = default;
    [SerializeField] TextMeshProUGUI timeLeftText = default;
    [SerializeField] Image resultIcon = default;

    public void SetArgumentNumberText(int argumentNumber)
    {
        argumentNumberText.text = "#" + argumentNumber + ":";
    }

    public void SetTimeLeftText(float timeLeft)
    {
        timeLeftText.text = (int)timeLeft + "\"";
    }

    public void SetResultIcon(Sprite sprite)
    {
        resultIcon.sprite = sprite;
    }
}