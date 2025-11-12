using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "NewCustomerCase", menuName = "Game/Customer Case")]
public class CustomerCase : ScriptableObject
{
    public enum CaseType { Ghost, Stalker }

    [Header("Case Info")]
    public CaseType caseType;
    public string caseName;


    [Header("Case Evidence")]
    public EvidencePhoto[] evidencePhotos;

    [Header("Solution")]
    public bool acceptsAnyTalisman = false;
    public ItemData[] requiredItems;
    public string failureOutcome;
    public string successOutcome;

    [Header("Unlocking")]
    public int unlockDay; // <-- Add this here

}

