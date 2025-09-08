using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Inventory Slots")]
    public SpriteRenderer[] inventorySlots;

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

        inventorySlots[nextSlotIndex].sprite = itemIcon;
        inventorySlots[nextSlotIndex].enabled = true;

        // Assign index to slot so it knows where it belongs
        InventorySlot slotScript = inventorySlots[nextSlotIndex].GetComponent<InventorySlot>();
        if (slotScript != null) slotScript.SetSlotIndex(nextSlotIndex);

        nextSlotIndex++;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= inventorySlots.Length) return;

        inventorySlots[index].sprite = null;
        inventorySlots[index].enabled = false;

        Debug.Log("Removed item from slot " + index);

        RearrangeInventory();
    }

    private void RearrangeInventory()
    {
        int fillIndex = 0;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].sprite != null)
            {
                // Move sprite down if needed
                if (fillIndex != i)
                {
                    inventorySlots[fillIndex].sprite = inventorySlots[i].sprite;
                    inventorySlots[fillIndex].enabled = true;

                    inventorySlots[i].sprite = null;
                    inventorySlots[i].enabled = false;
                }

                InventorySlot slotScript = inventorySlots[fillIndex].GetComponent<InventorySlot>();
                if (slotScript != null) slotScript.SetSlotIndex(fillIndex);

                fillIndex++;
            }
        }

        nextSlotIndex = fillIndex;
    }
}
