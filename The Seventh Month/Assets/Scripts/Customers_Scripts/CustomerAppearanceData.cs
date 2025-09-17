using UnityEngine;

[System.Serializable]
public class CustomerAppearanceData
{
    public int bodyIndex, hairBackIndex, hairFrontIndex, clothesIndex, eyesIndex, noseIndex, lipsIndex;
    public int glassesIndex;
    public CustomerAppearance.Gender gender; // store Male or Female


    // Colors
    public Color skinColor;
    public Color hairColor;
    public Color lipsColor;
    public Color clothesColor;
}


