using UnityEngine;
using UnityEngine.UI;

public class PhotoPanelManager : MonoBehaviour
{
    public GameObject expandedPanel;   // Panel with the photos
    public Button thumbnailButton;     // Click to open
    public Button backgroundButton;    // Static black background
    public float openDuration = 0.3f;

    [Header("Photo Slots")]
    public Image[] photoSlots; // Drag 3 UI Image components here in the Inspector

    void Start()
    {
        expandedPanel.SetActive(false);
        backgroundButton.gameObject.SetActive(false);

        thumbnailButton.onClick.AddListener(OpenPanel);
        backgroundButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        expandedPanel.SetActive(true);
        backgroundButton.gameObject.SetActive(true);

        // Reset scale before animation
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

    // 👉 NEW METHOD to actually show the photos
    public void ShowEvidencePhotos(Sprite[] photos)
    {
        for (int i = 0; i < photoSlots.Length; i++)
        {
            if (i < photos.Length)
            {
                photoSlots[i].sprite = photos[i];
                photoSlots[i].gameObject.SetActive(true);
            }
            else
            {
                photoSlots[i].gameObject.SetActive(false); // hide unused slots
            }
        }
    }
}
