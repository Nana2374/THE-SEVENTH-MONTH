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


    // Shows photos with torn masks based on current day.
    public void ShowEvidencePhotos(EvidencePhoto[] evidences)
    {
        // Determine how many photos to damage
        int damageCount = (currentDay == 1) ? Mathf.Min(3, evidences.Length) : evidences.Length;
        List<int> damagedIndices = new List<int>();

        // Weighted selection for day 1: prefer index 1 (undamaged)
        while (damagedIndices.Count < damageCount)
        {
            int index;
            if (currentDay == 1)
            {
                // Higher chance to pick undamaged photo
                float r = Random.value;
                Debug.Log("r value" + r);

                if (r < 0.9f && evidences.Length > 1)
                    index = 0;
                //index = Random.Range(0, evidences.Length);


                else
                    index = Random.Range(0, evidences.Length);
                //index = 0;
            }
            else
            {
                // Later days: pick any index
                index = Random.Range(0, evidences.Length);
            }

            if (!damagedIndices.Contains(index))
                damagedIndices.Add(index);
        }

        for (int i = 0; i < photoSlots.Length; i++)
        {
            if (i < evidences.Length)
            {
                var slot = photoSlots[i];

                // Apply damage only if this index is in the list
                if (damageOverlays.Length > 0 && damagedIndices.Contains(i))
                {
                    int randomIndex = Random.Range(0, damageOverlays.Length);
                    slot.maskImage.sprite = damageOverlays[randomIndex];
                    slot.maskImage.gameObject.SetActive(true);
                }
                else
                {
                    slot.maskImage.gameObject.SetActive(false); // undamaged
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
