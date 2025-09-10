using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerCase", menuName = "Game/Customer Case")]
public class CustomerCase : ScriptableObject
{
    public enum CaseType { Ghost, Stalker } // add more types if needed

    [Header("Case Info")]
    public CaseType caseType;

    [Header("Case Info")]
    public string caseName;               // e.g. "Orang Minyak Encounter"
    [TextArea] public string description; // What the customer says or shows
    public Sprite[] evidencePhotos;       // Evidence pictures

    [Header("Solution")]
    public ItemData[] requiredItems;      // Items needed to solve the case
    public string failureOutcome;         // What happens if unsolved
    public string successOutcome;         // What happens if solved
}

