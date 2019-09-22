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

        Button[] cluesButtons = inventoryItemsScreen.ItemsButtons.ToArray();

        for (int i = 0; i < cluesButtons.Length; i++)
        {
            InventoryItemInfo itemInfo = GetInventoryItemInfo(i);
            cluesButtons[i].onClick.AddListener(() => ChangeCurrentlySelectedItem(itemInfo));
        }
    }

    void ChangeCurrentlySelectedItem(InventoryItemInfo itemInfo)
    {
        currentlySelectedItem = itemInfo;
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

        inventoryAnimatedScreen.Show();
    }

    public void ChooseItem()
    {
        ChapterManager.Instance.SetPauseAvailability(enable: true);
        CharacterManager.Instance.PlayerController.Enable();
        GameManager.Instance.SetCursorEnable(enable: false);
        inventoryAnimatedScreen.Hide();
        onInventoryItemChosen.Invoke();
    }

    public void CancelItemUsage()
    {
        ChapterManager.Instance.SetPauseAvailability(enable: true);
        CharacterManager.Instance.PlayerController.Enable();
        GameManager.Instance.SetCursorEnable(enable: false);
        inventoryAnimatedScreen.Hide();
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
        get 
        {
            InventoryItemInfo item = currentlySelectedItem;
            currentlySelectedItem = null;
            
            return item;
        }
    }

    #endregion
}