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

        // Hide all slots at start
        HideAllSlots();
    }

    /// <summary>
    /// Add an item to the first empty slot
    /// </summary>
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

    /// <summary>
    /// Remove item at slot index
    /// </summary>
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

    /// <summary>
    /// Get a copy of current inventory items
    /// </summary>
    public List<ItemData> GetCurrentItemsData()
    {
        return new List<ItemData>(inventoryItems);
    }

    /// <summary>
    /// Check if inventory is full
    /// </summary>
    public bool IsFull()
    {
        return inventoryItems.Count >= inventorySlots.Length;
    }

    /// <summary>
    /// Return list of current item names for solution checking
    /// </summary>
    public List<string> GetCurrentItemNames()
    {
        List<string> names = new List<string>();
        foreach (var item in inventoryItems)
            names.Add(item.itemName);
        return names;
    }

    /// <summary>
    /// Update all inventory UI slots
    /// </summary>
    private void UpdateInventoryUI()
    {
        // First, hide all slots
        HideAllSlots();

        // Then, populate slots with items
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (i >= inventorySlots.Length) break;

            inventorySlots[i].sprite = inventoryItems[i].itemSprite;
            inventorySlots[i].enabled = true;
            inventorySlots[i].gameObject.SetActive(true);

            // Assign slot index for any InventorySlot scripts
            InventorySlot slotScript = inventorySlots[i].GetComponent<InventorySlot>();
            if (slotScript != null)
                slotScript.SetSlotIndex(i);
        }
    }

    /// <summary>
    /// Hide all inventory slots completely
    /// </summary>
    private void HideAllSlots()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot != null)
            {
                slot.sprite = null;
                slot.enabled = false;
                slot.gameObject.SetActive(false);
            }
        }
    }
}
