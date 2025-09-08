using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;      // e.g. "Match"
    public Sprite itemSprite;    // image to show in drawer or inventory
    public string description;   // optional
}
