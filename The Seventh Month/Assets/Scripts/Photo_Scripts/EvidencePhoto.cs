using UnityEngine;

[System.Serializable]
public class EvidencePhoto
{
    public Sprite photo;             // The main photo image
    [TextArea] public string caption;  // Optional caption for the photo
    public int dayTaken = 1;         // Which day the photo was taken
}
