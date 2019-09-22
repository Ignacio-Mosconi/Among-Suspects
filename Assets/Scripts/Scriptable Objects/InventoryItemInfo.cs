using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item Info", menuName = "Inventory Item Info", order = 3)]
public class InventoryItemInfo : ScriptableObject
{
    public string itemName;
    [TextArea(3, 10)] public string description;
    public Sprite itemSprite;
}