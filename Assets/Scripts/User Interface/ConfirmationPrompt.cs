using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class ConfirmationPrompt : MonoBehaviour
{
    [SerializeField] Button confirmButton = default;
    [SerializeField] Button cancelButton = default;
    [SerializeField] TextMeshProUGUI warningText = default;

    void Start()
    {
        confirmButton.onClick.AddListener(HideConfirmation);
        cancelButton.onClick.AddListener(HideConfirmation);
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