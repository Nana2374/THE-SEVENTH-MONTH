using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerCase", menuName = "Game/Customer Case")]
public class CustomerCase : ScriptableObject
{
    [Header("Case Info")]
    public string caseName;              // e.g. "Orang Minyak Encounter"
    [TextArea] public string description; // What the customer says or shows
    public Sprite[] evidencePhotos;      // Evidence picture shown to player

    [Header("Solution")]
    public string[] requiredItems;        // Items needed to solve (names or IDs)
    public string failureOutcome;         // What happens if unsolved
    public string successOutcome;         // What happens if solved
}
