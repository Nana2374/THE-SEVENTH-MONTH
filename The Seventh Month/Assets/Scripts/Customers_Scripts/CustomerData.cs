using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // <-- this makes it show up in the Inspector
public class CustomerData
{
    public string customerName;         // Add this line
    public GameObject customerPrefab;    // Drag your prefab here
    public CustomerCase[] possibleCases; // Drag your ScriptableObject cases here
}
