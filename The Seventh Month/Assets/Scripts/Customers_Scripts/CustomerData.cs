using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // still serializable, shows up in inspector
public class CustomerData
{
    public string customerName;          // Optional: give each customer a name
    public GameObject customerPrefab;    // Drag your prefab here
    public Sprite posterSprite;          //  Add this field for the bulletin board poster
    public CustomerCase[] possibleCases; // Drag your ScriptableObject cases here
}

