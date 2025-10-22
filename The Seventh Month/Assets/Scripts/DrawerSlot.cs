using UnityEngine;

public class DrawerSlot : MonoBehaviour
{
    public ItemData itemData;         // assign the ScriptableObject in the Inspector
    public SpriteRenderer itemSprite; // optional: shows the item in the drawer visually

    public AudioClip fullSound;
    public AudioSource audioSource;   // assign in Inspector

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
        if (itemData == null || InventoryManager.instance == null) return;

        if (InventoryManager.instance.IsFull())
        {
            Debug.Log("Inventory is full! Remove an item before adding.");
            if (fullSound != null && audioSource != null)
                audioSource.PlayOneShot(fullSound);
            return;
        }

        // Add item to inventory
        InventoryManager.instance.AddItem(itemData);
        Debug.Log("Added " + itemData.itemName + " to inventory");
    }
}




