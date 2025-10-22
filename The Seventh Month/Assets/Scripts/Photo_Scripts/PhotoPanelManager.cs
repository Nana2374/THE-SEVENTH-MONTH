using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PhotoPanelManager : MonoBehaviour
{
    [Header("Panel Setup")]
    public GameObject expandedPanel;
    public Button thumbnailButton;
    public Button backgroundButton;
    public float openDuration = 0.3f;

    [Header("Photo Slots")]
    public PhotoSlotUI[] photoSlots; // Custom class combining Mask and PhotoImage

    [Header("Damage Options")]
    public Sprite[] damageOverlays;   // Torn PNGs for masking
    public Color scribbleColor = Color.red;
    public string[] scribbleWords = { "??", "LIAR", "FAKE", "WHO?" };

    public int currentDay = 1;

    void Start()
    {
        expandedPanel.SetActive(false);
        backgroundButton.gameObject.SetActive(false);
        thumbnailButton.gameObject.SetActive(false);

        thumbnailButton.onClick.AddListener(OpenPanel);
        backgroundButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        expandedPanel.SetActive(true);
        backgroundButton.gameObject.SetActive(true);
        expandedPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(expandedPanel, Vector3.one, openDuration).setEaseOutBack();
    }

    public void ClosePanel()
    {
        LeanTween.scale(expandedPanel, Vector3.zero, openDuration).setEaseInBack()
            .setOnComplete(() =>
            {
                expandedPanel.SetActive(false);
                backgroundButton.gameObject.SetActive(false);
            });
    }

    // ✅ Weighted random helper
    int GetWeightedIndex(float[] weights)
    {
        float total = 0f;
        foreach (float w in weights)
            total += w;

        float randomValue = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (randomValue <= cumulative)
                return i;
        }

        return weights.Length - 1; // Fallback
    }

    // Shows photos with torn masks based on current day.
    public void ShowEvidencePhotos(EvidencePhoto[] evidences)
    {
        for (int i = 0; i < photoSlots.Length; i++)
        {
            if (i < evidences.Length)
            {
                var slot = photoSlots[i];

                // Weighted probability
                int damageIndex = 0;

                float roll = Random.value; // gives a float between 0.0 and 1.0

                switch (currentDay)
                {
                    case 1:
                        // Day 1: almost always undamaged
                        if (roll < 0.9f) damageIndex = 0;       // 90%
                        else if (roll < 0.97f) damageIndex = 1; // 7%
                        else if (roll < 0.995f) damageIndex = 2; // 2.5%
                        else damageIndex = 3;                    // 0.5%
                        break;

                    case 2:
                        // Day 2: light damage appears more
                        if (roll < 0.6f) damageIndex = 0;       // 60%
                        else if (roll < 0.85f) damageIndex = 1; // 25%
                        else if (roll < 0.95f) damageIndex = 2; // 10%
                        else damageIndex = 3;                   // 5%
                        break;

                    case 3:
                        // Day 3: broader damage spread
                        if (roll < 0.4f) damageIndex = 0;       // 40%
                        else if (roll < 0.7f) damageIndex = 1;  // 30%
                        else if (roll < 0.9f) damageIndex = 2;  // 20%
                        else damageIndex = 3;                   // 10%
                        break;

                    case 4:
                        // Day 4: heavily damaged photos more common
                        if (roll < 0.2f) damageIndex = 0;       // 20%
                        else if (roll < 0.5f) damageIndex = 1;  // 30%
                        else if (roll < 0.8f) damageIndex = 2;  // 30%
                        else damageIndex = 3;                   // 20%
                        break;

                    default:
                        // Later days: evenly distributed damage
                        if (roll < 0.25f) damageIndex = 0;
                        else if (roll < 0.5f) damageIndex = 1;
                        else if (roll < 0.75f) damageIndex = 2;
                        else damageIndex = 3;
                        break;
                }


                // Apply damage overlay
                if (damageOverlays.Length > 0 && damageIndex < damageOverlays.Length)
                {
                    slot.maskImage.sprite = damageOverlays[damageIndex];
                    slot.maskImage.gameObject.SetActive(true);
                }
                else
                {
                    slot.maskImage.gameObject.SetActive(false);
                }

                // Set original photo
                slot.photoImage.sprite = evidences[i].photo;
                slot.photoImage.gameObject.SetActive(true);

                // Set caption
                if (slot.captionText != null)
                {
                    slot.captionText.text = evidences[i].caption;
                    slot.captionText.gameObject.SetActive(true);

                    // Optional scribble for older photos
                    if (currentDay >= 1)
                    {
                        string scribble = scribbleWords[Random.Range(0, scribbleWords.Length)];
                        slot.captionText.text += $"\n<color=#{ColorUtility.ToHtmlStringRGB(scribbleColor)}>{scribble}</color>";
                    }
                }
            }
            else
            {
                // Hide unused slots
                photoSlots[i].photoImage.gameObject.SetActive(false);
                if (photoSlots[i].captionText != null)
                    photoSlots[i].captionText.gameObject.SetActive(false);
                photoSlots[i].maskImage.gameObject.SetActive(false);
            }
        }
    }

    public void ShowThumbnail() => thumbnailButton.gameObject.SetActive(true);
    public void HideThumbnail() => thumbnailButton.gameObject.SetActive(false);
}

/// Helper class to combine mask and photo image
[System.Serializable]
public class PhotoSlotUI
{
    public Image maskImage;      // Torn PNG with Mask component
    public Image photoImage;     // Original photo (child of maskImage)
    public TextMeshProUGUI captionText;
}
