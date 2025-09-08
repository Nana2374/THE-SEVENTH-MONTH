using UnityEngine;

public class DrawerSlot : MonoBehaviour
{
    public ItemData itemData;         // assign the ScriptableObject in the Inspector
    public SpriteRenderer itemSprite; // optional: shows the item in the drawer visually

    void Start()
    {
        // Show the sprite in the drawer
        if (itemSprite != null && itemData != null)
        {
            itemSprite.sprite = itemData.itemSprite;
            itemSprite.gameObject.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        if (itemData == null) return;

        // Check if InventoryManager exists
        if (InventoryManager.instance == null) return;

        // Check if inventory is full
        if (InventoryManager.instance.IsFull())
        {
            Debug.Log("Inventory is full! Remove an item before adding.");
            // Optional: play sound, flash UI, or animate the slot
            return;
        }

        // Add item to inventory
        InventoryManager.instance.AddItem(itemData);
        Debug.Log("Added " + itemData.itemName + " to inventory");
    }
}


