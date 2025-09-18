using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // <-- this makes it show up in the Inspector
public class CustomerData
{
    public string customerName;         // Add this line
    public Sprite customerSprite;  // <--- ADD SPRITE
    public Sprite failureSprite;       // Appearance if failed previously
    public CustomerCase[] possibleCases; // Drag your ScriptableObject cases here

    // Audio
    public AudioClip arrivalClip;
    public AudioClip departureClip;
}
