using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class ConfirmationPrompt : MonoBehaviour
{
    [SerializeField] GameObject mainArea = default;
    [SerializeField] Button confirmButton = default;
    [SerializeField] Button cancelButton = default;
    [SerializeField] TextMeshProUGUI warningText = default;

    void Start()
    {
        confirmButton.onClick.AddListener(ShowConfirmation);
        cancelButton.onClick.AddListener(HideConfirmation);
    }

    public void ShowConfirmation()
    {
        mainArea.SetActive(false);
        gameObject.SetActive(true);
    }

    public void HideConfirmation()
    {
        gameObject.SetActive(false);
        mainArea.SetActive(true);
    }

    public void AddConfirmationListener(Action action)
    {
        confirmButton.onClick.AddListener(() => action());
    }

    public void RemoveAllConfirmationListeners()
    {
        confirmButton.onClick.RemoveAllListeners();
    }

    public void AddCancelationListener(Action action)
    {
        cancelButton.onClick.AddListener(() => action());
    }

    public void ChangeWarningMessage(string message)
    {
        warningText.text = message;
    }
}