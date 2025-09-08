using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private SpriteRenderer sr;
    private int slotIndex;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    void OnMouseDown()
    {
        if (sr != null && sr.sprite != null)
        {
            InventoryManager.instance.RemoveItem(slotIndex);
        }
    }
}

