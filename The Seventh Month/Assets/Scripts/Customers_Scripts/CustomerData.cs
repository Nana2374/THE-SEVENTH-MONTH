using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] // <-- this makes it show up in the Inspector
public class CustomerData
{
    public string customerName;         // Add this line
    public Sprite customerSprite;  // <--- ADD SPRITE
    public Sprite failureSprite;       // Appearance if failed previously

    public enum Gender { Male, Female }
    public Gender customerGender; // 👈 New field for voice logic


    [Header("Failure Assets")]
    public Sprite failurePoster;    // Moved here! Customer-specific failure poster


    [Header("Case Data")]
    [TextArea] public string caseDescription;   // Moved from CustomerCase
    [TextArea] public string failureDialogue;   // Moved from CustomerCase



    public CustomerCase[] possibleCases; // Drag your ScriptableObject cases here

    // Audio
    public AudioClip arrivalClip;
    public AudioClip departureClip;


}
