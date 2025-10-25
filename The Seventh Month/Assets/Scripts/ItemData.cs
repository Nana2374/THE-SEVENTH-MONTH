using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public enum ItemCategory
    {
        General,
        Talisman       // covers Malay or Chinese talismans

    }


    [Header("Item Info")]
    public string itemName;      // e.g. "Match"
    public Sprite itemSprite;    // image to show in drawer or inventory
    public string description;   // optional

    [Header("Item Category")]
    public ItemCategory category = ItemCategory.General;
}
