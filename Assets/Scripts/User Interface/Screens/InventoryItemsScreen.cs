using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItemsScreen : MonoBehaviour
{
    [SerializeField] GameObject itemsButtonsPanel = default;
    [SerializeField] GameObject itemsDescriptionArea = default;
    [SerializeField] TextMeshProUGUI itemTitleText = default;
    [SerializeField] TextMeshProUGUI itemDescriptionText = default;
    [SerializeField] Image itemImage = default;

    List<Button> itemsButtons = new List<Button>();
    RectTransform itemsButtonsPanelsRectTrans;
    GameObject itemButtonPrefab;
    Button lastButtonSelected;
    Language previousLanguage;
    float addeditemButtonsPanelSize;

    const string itemButtonPrefabPath = "Menu Elements/Item Button";

    void Awake()
    {
        GridLayoutGroup itemsButtonsPanelGridLay = itemsButtonsPanel.GetComponent<GridLayoutGroup>();
        
        itemsButtonsPanelsRectTrans = itemsButtonsPanel.GetComponent<RectTransform>();
        
        itemButtonPrefab = Resources.Load(itemButtonPrefabPath) as GameObject;
        itemsButtonsPanelsRectTrans.sizeDelta = new Vector2(itemsButtonsPanelsRectTrans.sizeDelta.x,
                                                            itemsButtonsPanelGridLay.padding.top + itemsButtonsPanelGridLay.padding.bottom);

        int itemsAmount = InventoryManager.Instance.InventoryItemsAmount;
        float itemButtonHeight = itemButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        
        addeditemButtonsPanelSize = itemButtonHeight + itemsButtonsPanelGridLay.spacing.y;

        for (int i = 0; i < itemsAmount; i++)
        {
            GameObject itemButtonObject = Instantiate(itemButtonPrefab, itemsButtonsPanel.transform);
            Button itemButton = itemButtonObject.GetComponent<Button>();
            Image buttonImage = itemButton.transform.GetChild(0).GetComponent<Image>();
            InventoryItemInfo itemInfo = InventoryManager.Instance.GetInventoryItemInfo(i);

            buttonImage.sprite = itemInfo.itemSprite;

            itemButton.onClick.AddListener(() => SelectItem(itemInfo, itemButton));
            itemButton.gameObject.SetActive(false);
            itemsButtons.Add(itemButton);
        }

        previousLanguage = GameManager.Instance.CurrentLanguage;
    }

    void OnEnable()
    {
        if (previousLanguage != GameManager.Instance.CurrentLanguage)
            ReloadItemsInCurrentLanguage();

        int founditemIndex = 0;

        for (int i = 0; i < itemsButtons.Count; i++)
        {
            InventoryItemInfo itemInfo = InventoryManager.Instance.GetInventoryItemInfo(i);

            if (!itemsButtons[i].gameObject.activeSelf && InventoryManager.Instance.HasInventoryItem(ref itemInfo))
            {
                itemsButtons[i].gameObject.SetActive(true);
                itemsButtonsPanelsRectTrans.sizeDelta = new Vector2(itemsButtonsPanelsRectTrans.sizeDelta.x, 
                                                                    itemsButtonsPanelsRectTrans.sizeDelta.y + addeditemButtonsPanelSize);
            }

            if (itemsButtons[i].gameObject.activeSelf)
            {
                if (founditemIndex == 0)
                {
                    lastButtonSelected = itemsButtons[i];
                    GameManager.Instance.InvokeMethodInRealTime(itemsButtons[i].Select, 0.1f);
                    SelectItem(itemInfo, itemsButtons[i]);
                }

                itemsButtons[i].transform.SetSiblingIndex(founditemIndex);
                founditemIndex++;
            }
        }

        itemsDescriptionArea.SetActive(founditemIndex != 0);
    }

    void Update()
    {
        if (!EventSystem.current.currentSelectedGameObject && lastButtonSelected)
            EventSystem.current.SetSelectedGameObject(lastButtonSelected.gameObject);
    }

    void ReloadItemsInCurrentLanguage()
    {
        previousLanguage = GameManager.Instance.CurrentLanguage;

        for (int i = 0; i < itemsButtons.Count; i++)
        {
            Button itemButton = itemsButtons[i];
            InventoryItemInfo inventoryItemInfo = InventoryManager.Instance.GetInventoryItemInfo(i);

            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => SelectItem(inventoryItemInfo, itemButton));
        }
    }

    void SelectItem(InventoryItemInfo itemInfo, Button itemButton)
    {
        lastButtonSelected = itemButton;
        itemTitleText.text = itemInfo.itemName;
        itemDescriptionText.text = itemInfo.description;
        itemImage.sprite = itemInfo.itemSprite;
    }

    #region Properties

    public List<Button> ItemsButtons
    {
        get { return itemsButtons; }
    }

    #endregion
}