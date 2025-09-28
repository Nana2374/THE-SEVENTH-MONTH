using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    /// <summary>
    /// Shows photos with torn masks based on current day.
    /// </summary>
    public void ShowEvidencePhotos(EvidencePhoto[] evidences)
    {
        for (int i = 0; i < photoSlots.Length; i++)
        {
            if (i < evidences.Length)
            {
                var slot = photoSlots[i];

                // Assign a random torn mask
                if (damageOverlays.Length > 0)
                {
                    int randomIndex = Random.Range(0, damageOverlays.Length);
                    slot.maskImage.sprite = damageOverlays[randomIndex];
                }

                // Set the original photo under the mask
                slot.photoImage.sprite = evidences[i].photo;
                slot.photoImage.gameObject.SetActive(true);

                // Set caption
                if (slot.captionText != null)
                {
                    slot.captionText.text = evidences[i].caption;
                    slot.captionText.gameObject.SetActive(true);

                    // Optional scribble for older photos
                    if (currentDay >= 3)
                    {
                        string scribble = scribbleWords[Random.Range(0, scribbleWords.Length)];
                        slot.captionText.text += $"\n<color=#{ColorUtility.ToHtmlStringRGB(scribbleColor)}>{scribble}</color>";
                    }
                }

                // Ensure mask is active
                slot.maskImage.gameObject.SetActive(true);
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

/// <summary>
/// Helper class to combine mask and photo image
/// </summary>
[System.Serializable]
public class PhotoSlotUI
{
    public Image maskImage;      // Torn PNG with Mask component
    public Image photoImage;     // Original photo (child of maskImage)
    public TextMeshProUGUI captionText;
}
