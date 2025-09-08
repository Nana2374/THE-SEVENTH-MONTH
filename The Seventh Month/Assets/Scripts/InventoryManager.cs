using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Inventory Slots (SpriteRenderers)")]
    public SpriteRenderer[] inventorySlots;   // UI slots showing item sprites

    private List<ItemData> inventoryItems = new List<ItemData>();


    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    // Add item to first empty slot
    public void AddItem(ItemData itemData)
    {
        if (IsFull())
        {
            Debug.Log("Inventory full! Remove an item before adding new ones.");
            return;
        }

        inventoryItems.Add(itemData);
        UpdateInventoryUI();
    }

    // Remove item at slot index
    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryItems.Count)
        {
            Debug.LogWarning("Invalid inventory slot index");
            return;
        }

        inventoryItems.RemoveAt(slotIndex);
        UpdateInventoryUI();
    }

    public List<ItemData> GetCurrentItemsData()
    {
        return new List<ItemData>(inventoryItems); // return a copy of the list
    }


    // Check if inventory is full
    public bool IsFull()
    {
        return inventoryItems.Count >= inventorySlots.Length;
    }

    // Return list of current item names for solution checking
    public List<string> GetCurrentItemNames()
    {
        List<string> names = new List<string>();
        foreach (var item in inventoryItems)
            names.Add(item.itemName);
        return names;
    }

    // --- Update the UI sprites based on inventoryItems list ---
    private void UpdateInventoryUI()
    {
        // Clear all slots first
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].sprite = null;
            inventorySlots[i].enabled = false;
        }

        // Fill in current items
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            inventorySlots[i].sprite = inventoryItems[i].itemSprite;
            inventorySlots[i].enabled = true;

            InventorySlot slotScript = inventorySlots[i].GetComponent<InventorySlot>();
            if (slotScript != null)
                slotScript.SetSlotIndex(i);
        }
    }
}


