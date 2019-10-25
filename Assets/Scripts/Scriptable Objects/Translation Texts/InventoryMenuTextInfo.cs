using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Menu Text Info", menuName = "Inventory Menu Text Info", order = 9)]
public class InventoryMenuTextInfo : ScriptableObject
{
    [Header("Main Screen")]
    public string chooseItemTitle;
    public string useItemButtonText;
    public string cancelItemButtonText;
}