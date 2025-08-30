using UnityEngine;
using UnityEngine.UI;

public class PhotoPanelManager : MonoBehaviour
{
    public GameObject expandedPanel;   // Panel with the photos
    public Button thumbnailButton;     // Click to open
    public Button backgroundButton;    // Static black background
    public float openDuration = 0.3f;

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
}


