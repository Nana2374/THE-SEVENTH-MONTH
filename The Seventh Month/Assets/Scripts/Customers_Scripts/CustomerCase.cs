using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "NewCustomerCase", menuName = "Game/Customer Case")]
public class CustomerCase : ScriptableObject
{
    public enum CaseType { Ghost, Stalker }
    public enum gender { Male, Female }

    [Header("Case Info")]
    public CaseType caseType;
    public string caseName;
    [TextArea] public string description;


    [Header("Case Evidence")]
    public EvidencePhoto[] evidencePhotos;

    [Header("Solution")]
    public bool acceptsAnyTalisman = false;
    public ItemData[] requiredItems;
    public string failureOutcome;
    public string successOutcome;

    [Header("Failure Dialogue")]
    [TextArea] public string failureDialogue;

    public gender caseGender;

}

