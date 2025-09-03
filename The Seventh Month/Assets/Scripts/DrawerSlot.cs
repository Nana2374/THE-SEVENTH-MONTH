using UnityEngine;

public class DrawerSlot : MonoBehaviour
{
    public Sprite itemIcon;           // Sprite to add to inventory
    public SpriteRenderer itemSprite; // Child SpriteRenderer showing the item

    void Start()
    {
        // Show item in the drawer
        if (itemSprite != null && itemIcon != null)
        {
            itemSprite.sprite = itemIcon;
            itemSprite.gameObject.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        InventoryManager.instance.AddItem(itemIcon);
        // Optional: hide the item if you only allow taking once
        // itemSprite.gameObject.SetActive(false);
        Debug.Log("Item clicked!");
    }
}


