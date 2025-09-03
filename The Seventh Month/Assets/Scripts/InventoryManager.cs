using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Inventory Slots")]
    public SpriteRenderer[] inventorySlots; // world-space sprite slots

    private int nextSlotIndex = 0;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddItem(Sprite itemIcon)
    {
        if (nextSlotIndex >= inventorySlots.Length)
        {
            Debug.Log("Inventory full!");
            return;
        }

        // Set the sprite for the next available slot
        inventorySlots[nextSlotIndex].sprite = itemIcon;
        inventorySlots[nextSlotIndex].enabled = true; // make sure it's visible

        nextSlotIndex++;
        Debug.Log("Added item to slot " + nextSlotIndex);
    }
}

