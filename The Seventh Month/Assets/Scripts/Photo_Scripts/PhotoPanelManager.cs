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
    public Image[] photoSlots;
    public TextMeshProUGUI[] captionSlots;

    [Header("Degradation Settings")]

    public Sprite[] damageOverlays;   // Torn/ripped textures
    public float alphaPerDay = 0.25f; // Overlay visibility per day

    public Color scribbleColor = Color.red;
    public string[] scribbleWords = { "??", "LIAR", "FAKE", "WHO?" };

    public int currentDay = 1;



    void Start()
    {
        expandedPanel.SetActive(false);
        thumbnailButton.gameObject.SetActive(false);
        backgroundButton.gameObject.SetActive(false);

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

    public void ShowEvidencePhotos(EvidencePhoto[] evidences)
    {
        for (int i = 0; i < photoSlots.Length; i++)
        {
            if (i < evidences.Length)
            {
                Image photo = photoSlots[i];
                TextMeshProUGUI caption = i < captionSlots.Length ? captionSlots[i] : null;

                photo.sprite = evidences[i].photo;
                photo.gameObject.SetActive(true);

                if (caption != null)
                {
                    caption.text = evidences[i].caption;
                    caption.gameObject.SetActive(true);
                }

                ApplyDegradation(photo, caption, evidences[i]);
            }
            else
            {
                photoSlots[i].gameObject.SetActive(false);
                if (i < captionSlots.Length)
                    captionSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void ApplyDegradation(Image photo, TextMeshProUGUI caption, EvidencePhoto evidence)
    {
        int age = currentDay - evidence.dayTaken;
        if (age <= 0) return; // Fresh photo, no damage

        // Randomly decide how many tears to apply (1 to damageOverlays.Length)
        int tearsToApply = Random.Range(1, damageOverlays.Length + 1);

        for (int i = 0; i < tearsToApply; i++)
        {
            // Random overlay name
            string overlayName = $"Overlay_{i}_{Random.Range(0, 1000)}";

            GameObject overlayGO = new GameObject(overlayName);
            overlayGO.transform.SetParent(photo.transform, false);

            Image overlay = overlayGO.AddComponent<Image>();
            overlay.sprite = damageOverlays[i % damageOverlays.Length];
            overlay.raycastTarget = false;

            // Random position & rotation
            RectTransform rt = overlayGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            float randomX = Random.Range(-0.2f, 0.2f);
            float randomY = Random.Range(-0.2f, 0.2f);
            rt.localPosition = new Vector3(randomX * photo.rectTransform.rect.width,
                                           randomY * photo.rectTransform.rect.height, 0f);
            rt.localRotation = Quaternion.Euler(0, 0, Random.Range(-30f, 30f));
            rt.localScale = Vector3.one * Random.Range(0.8f, 1.2f);

            // Alpha based on age
            float alpha = Mathf.Clamp01(age * alphaPerDay);
            overlay.color = new Color(1, 1, 1, alpha);
        }

        // Optional: scribbled captions for older photos
        if (caption != null && age >= 3)
        {
            string scribble = scribbleWords[Random.Range(0, scribbleWords.Length)];
            caption.text += $"\n<color=#{ColorUtility.ToHtmlStringRGB(scribbleColor)}>{scribble}</color>";
        }
    }


    public void ShowThumbnail() => thumbnailButton.gameObject.SetActive(true);
    public void HideThumbnail() => thumbnailButton.gameObject.SetActive(false);
}
