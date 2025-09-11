using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PhotoEvidence
{
    public Sprite photo;
    [TextArea] public string caption;
}

[CreateAssetMenu(fileName = "NewCustomerCase", menuName = "Game/Customer Case")]
public class CustomerCase : ScriptableObject
{
    public enum CaseType { Ghost, Stalker } // add more types if needed

    [Header("Case Info")]
    public CaseType caseType;

    [Header("Case Info")]
    public string caseName;               // e.g. "Orang Minyak Encounter"
    [TextArea] public string description; // What the customer says or shows


    [Header("Case Evidence")]
    public PhotoEvidence[] evidencePhotos;  // Photos with captions



    [Header("Solution")]
    public ItemData[] requiredItems;      // Items needed to solve the case
    public string failureOutcome;         // What happens if unsolved
    public string successOutcome;         // What happens if solved
}

