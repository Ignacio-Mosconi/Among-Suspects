using UnityEngine;
using TMPro;

public class InventoryMenu : Menu
{
    [Header("Translation Texts")]
    [SerializeField] TextMeshProUGUI chooseItemTitleText = default;
    [SerializeField] TextMeshProUGUI useItemButtonText = default;
    [SerializeField] TextMeshProUGUI cancelItemButtonText = default;

    protected override void SetUpTexts()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        InventoryMenuTextInfo inventoryMenuTextInfo = menuTextsByLanguage[language] as InventoryMenuTextInfo;

        if (!inventoryMenuTextInfo)
        {
            Debug.LogError("The scriptable object set for translation is incorrect.", gameObject);
            return;
        }

        chooseItemTitleText.text = inventoryMenuTextInfo.chooseItemTitle;
        useItemButtonText.text = inventoryMenuTextInfo.useItemButtonText;
        cancelItemButtonText.text = inventoryMenuTextInfo.cancelItemButtonText;
    }
}