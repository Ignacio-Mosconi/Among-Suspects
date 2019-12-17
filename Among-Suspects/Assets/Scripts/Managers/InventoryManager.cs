using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    #region Singleton

    static InventoryManager instance;

    void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(transform.parent.gameObject);
    }

    public static InventoryManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<InventoryManager>();
                if (!instance)
                    Debug.LogError("There is no 'Inventory Manager' in the scene");
            }

            return instance;
        }
    }

    #endregion

    [SerializeField] InventoryItemsScreen inventoryItemsScreen = default;
    [SerializeField] Button useItemButton = default;

    List<InventoryItemInfo> collectedInventoryItems = new List<InventoryItemInfo>();
    InventoryItemInfo[] allInventoryItems;
    InventoryItemInfo currentlySelectedItem;
    AnimatedMenuScreen inventoryAnimatedScreen;

    UnityEvent onInventoryItemChosen = new UnityEvent();

    void Start()
    {
        inventoryAnimatedScreen = inventoryItemsScreen.GetComponent<AnimatedMenuScreen>();
        inventoryAnimatedScreen.SetUp();
        
        LoadInventoryItems();
        GameManager.Instance.OnLanguageChanged.AddListener(LoadInventoryItems);

        inventoryItemsScreen.OnItemDeselected.AddListener(() =>
        {
            ExtendedInputModule inputModule = EventSystem.current.GetComponent<ExtendedInputModule>();

            if (inputModule.GetHoveredObject() != useItemButton.GetComponentInChildren<TextMeshProUGUI>().gameObject)
                ChangeCurrentlySelectedItem(null);
        });

        useItemButton.interactable = false;
    }

    void LoadInventoryItems()
    {
        Language language = GameManager.Instance.CurrentLanguage;
        string languagePath = Enum.GetName(typeof(Language), language);

        allInventoryItems = Resources.LoadAll<InventoryItemInfo>("Inventory Items/" + languagePath);

        foreach (InventoryItemInfo inventoryItemInfo in allInventoryItems)
        {
            InventoryItemInfo inventoryItemInfoInList = collectedInventoryItems.Find(it => it.itemID == inventoryItemInfo.itemID);

            if (inventoryItemInfoInList)
            {
                collectedInventoryItems.Add(inventoryItemInfo);
                collectedInventoryItems.Remove(inventoryItemInfoInList);
            }
        }
    }

    void ChangeCurrentlySelectedItem(InventoryItemInfo itemInfo)
    {
        currentlySelectedItem = itemInfo;
        useItemButton.interactable = (itemInfo != null);
    }

    void HideItemSelectionScreen()
    {
        Button[] itemButtons = inventoryItemsScreen.ItemsButtons.ToArray();

        foreach (Button itemButton in itemButtons)
            itemButton.onClick.RemoveAllListeners();

        ChapterManager.Instance.SetPauseAvailability(enable: true);
        CharacterManager.Instance.PlayerController.Enable();
        GameManager.Instance.SetCursorEnable(enable: false);
        inventoryAnimatedScreen.Hide();
    }

    public InventoryItemInfo GetInventoryItemInfo(int index)
    {
        InventoryItemInfo inventoryItem = null;

        if (index < allInventoryItems.Length && index >= 0)
            inventoryItem = allInventoryItems[index];

        return inventoryItem;
    }

    public void ShowItemSelectionScreen()
    {
        ChapterManager.Instance.SetPauseAvailability(enable: false);
        CharacterManager.Instance.PlayerController.Disable();
        GameManager.Instance.SetCursorEnable(enable: true);
        
        inventoryAnimatedScreen.Show();
        useItemButton.interactable = false;

        Button[] itemButtons = inventoryItemsScreen.ItemsButtons.ToArray();

        for (int i = 0; i < itemButtons.Length; i++)
        {
            InventoryItemInfo itemInfo = GetInventoryItemInfo(i);
            Button itemButton = itemButtons[i];
            UnityAction listener = () => ChangeCurrentlySelectedItem(itemInfo);
            
            itemButton.onClick.AddListener(listener);
        }
    }

    public void ChooseItem()
    {
        HideItemSelectionScreen();
        onInventoryItemChosen.Invoke();
    }

    public void CancelItemUsage()
    {
        HideItemSelectionScreen();
    }

    public void AddInventoryItem(InventoryItemInfo itemInfo)
    {
        if (!collectedInventoryItems.Find(it => it.itemID == itemInfo.itemID))
        {
            InventoryItemInfo itemInfoToAdd = Array.Find(allInventoryItems, it => it.itemID == itemInfo.itemID); 
            
            CharacterManager.Instance.PlayerController.OnItemCollected.Invoke();
            collectedInventoryItems.Add(itemInfoToAdd);
        }
    }

    public bool HasInventoryItem(ref InventoryItemInfo itemInfo)
    {
        bool hasItem = collectedInventoryItems.Contains(itemInfo);

        return hasItem;
    }

    public void ResetInventory()
    {
        collectedInventoryItems.Clear();
        LoadInventoryItems();
    }

    #region Properties

    public int InventoryItemsAmount
    {
        get { return allInventoryItems.Length; }
    }

    public UnityEvent OnInventoryItemChosen
    {
        get { return onInventoryItemChosen; }
    }

    public InventoryItemInfo CurrentlySelectedItem
    {
        get { return currentlySelectedItem; }
    }

    #endregion
}