using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationPrompt : MonoBehaviour
{
    [SerializeField] Button confirmButton = default;
    [SerializeField] Button cancelButton = default;
    [SerializeField] TextMeshProUGUI warningText = default;
    [SerializeField] string[] affirmativeTexts = new string[(int)Language.Count];
    [SerializeField] string[] negativeTexts = new string[(int)Language.Count];

    TextMeshProUGUI affirmativeButtonText = default;
    TextMeshProUGUI negativeButtonText = default;

    void Awake()
    {
        affirmativeButtonText = confirmButton.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
        negativeButtonText = cancelButton.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
    }

    void Start()
    {
        ChangeButtonsLanguage();

        confirmButton.onClick.AddListener(HideConfirmation);
        cancelButton.onClick.AddListener(HideConfirmation);
        
        GameManager.Instance.AddCursorPointerEvents(confirmButton);
        GameManager.Instance.AddCursorPointerEvents(cancelButton);
        GameManager.Instance.OnLanguageChanged.AddListener(ChangeButtonsLanguage);
    }

    void ChangeButtonsLanguage()
    {
        Language language = GameManager.Instance.CurrentLanguage;

        affirmativeButtonText.text = affirmativeTexts[(int)language];
        negativeButtonText.text = negativeTexts[(int)language];
    }

    public void ShowConfirmation()
    {
        gameObject.SetActive(true);
    }

    public void HideConfirmation()
    {
        gameObject.SetActive(false);
    }

    public void AddConfirmationListener(Action action)
    {
        confirmButton.onClick.AddListener(() => action());
    }

    public void RemoveAllConfirmationListeners()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(HideConfirmation);
    }

    public void AddCancelationListener(Action action)
    {
        cancelButton.onClick.AddListener(() => action());
    }

    public void RemoveAllCancelationListeners()
    {
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(HideConfirmation);
    }

    public void ChangeWarningMessage(string message)
    {
        warningText.text = message;
    }
}