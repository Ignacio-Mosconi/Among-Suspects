using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIPrompt))]
public class StarRating : MonoBehaviour
{
    [SerializeField] Image starFill = default;

    UIPrompt uiPrompt;

    public void SetUpPrompt()
    {
        uiPrompt = GetComponent<UIPrompt>();
        uiPrompt.SetUp();
    }

    public void ShowStar()
    {
        starFill.gameObject.SetActive(true);
        uiPrompt.Show();
    }

    public void HideStar()
    {
        starFill.gameObject.SetActive(false);
    }

    #region Properties

    public UIPrompt UIPrompt
    {
        get { return uiPrompt; }
    }

    #endregion
}