using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCustomerCase", menuName = "Game/Customer Case")]
public class CustomerCase : ScriptableObject
{
    public enum CaseType { Ghost, Stalker }

    [Header("Case Info")]
    public CaseType caseType;
    public string caseName;
    [TextArea] public string description;

    [Header("Case Evidence")]
    public EvidencePhoto[] evidencePhotos;  // Use EvidencePhoto array

    [Header("Solution")]
    public ItemData[] requiredItems;
    public string failureOutcome;
    public string successOutcome;

    [Header("Failure Poster")]
    public Sprite failurePoster;

    [Header("Failure Dialogue")]
    [TextArea] public string failureDialogue; // Line that plays when customer returns after failing

}

