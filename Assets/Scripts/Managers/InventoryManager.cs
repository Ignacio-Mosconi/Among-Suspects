using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

    List<InventoryItemInfo> collectedInventoryItems = new List<InventoryItemInfo>();
    InventoryItemInfo[] allInventoryItems;
    InventoryItemInfo currentlySelectedItem;
    AnimatedMenuScreen inventoryAnimatedScreen;

    UnityEvent onInventoryItemChosen = new UnityEvent();

    void Start()
    {
        allInventoryItems = Resources.LoadAll<InventoryItemInfo>("Inventory Items");
        inventoryAnimatedScreen = inventoryItemsScreen.GetComponent<AnimatedMenuScreen>();
        inventoryAnimatedScreen.SetUp();
    }

    void ChangeCurrentlySelectedItem(InventoryItemInfo itemInfo)
    {
        currentlySelectedItem = itemInfo;
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

        if (collectedInventoryItems.Count > 0)
        {
            for (int i = 0; i < InventoryItemsAmount; i++)
            {
                InventoryItemInfo itemInfo = GetInventoryItemInfo(i);

                if (HasInventoryItem(ref itemInfo))
                {
                    ChangeCurrentlySelectedItem(itemInfo);
                    break;
                }
            }
        }

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
        if (!collectedInventoryItems.Contains(itemInfo))
            collectedInventoryItems.Add(itemInfo);
    }

    public bool HasInventoryItem(ref InventoryItemInfo itemInfo)
    {
        bool hasItem = collectedInventoryItems.Contains(itemInfo);

        return hasItem;
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