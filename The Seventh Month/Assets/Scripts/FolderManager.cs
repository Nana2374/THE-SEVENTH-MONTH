using UnityEngine;
using UnityEngine.UI;

public class FolderManager : MonoBehaviour
{
    [Header("Folder Elements")]
    public Button folderButton;          // Closed folder tab
    public GameObject expandedFolder;    // Panel containing left + right pages
    public Button backgroundButton;      // Invisible full-screen button for closing

    [Header("Pages")]
    public GameObject leftPage;          // Static left page
    public GameObject[] rightPages;      // Only the right pages

    [Header("Animation Settings")]
    public bool useScaleAnimation = true;  // Set false to skip scaling
    public float openDuration = 0.3f;
    public float closeDuration = 0.2f;
    public Vector3 foldedScale = Vector3.one; // Scale when “closed” (small tab)
    public Vector3 openScale = Vector3.one;   // Scale when open (designed size)

    private int currentRightPage = 0;

    void Start()
    {
        // Ensure folder is closed initially
        expandedFolder.SetActive(false);
        backgroundButton.gameObject.SetActive(false);
        expandedFolder.transform.localScale = foldedScale;

        // Setup button listeners
        folderButton.onClick.AddListener(OpenFolder);
        backgroundButton.onClick.AddListener(CloseFolder);

        ShowRightPage(0); // Show first right page by default
    }

    public void OpenFolder()
    {
        expandedFolder.SetActive(true);
        backgroundButton.gameObject.SetActive(true);

        if (useScaleAnimation)
        {
            LeanTween.scale(expandedFolder, openScale, openDuration).setEaseOutBack();
        }
        else
        {
            expandedFolder.transform.localScale = openScale;
        }
    }

    public void CloseFolder()
    {
        if (useScaleAnimation)
        {
            LeanTween.scale(expandedFolder, foldedScale, closeDuration).setEaseInBack()
                .setOnComplete(() =>
                {
                    expandedFolder.SetActive(false);
                    backgroundButton.gameObject.SetActive(false);
                });
        }
        else
        {
            expandedFolder.transform.localScale = foldedScale;
            expandedFolder.SetActive(false);
            backgroundButton.gameObject.SetActive(false);
        }
    }

    public void ShowRightPage(int index)
    {
        if (index == currentRightPage || index < 0 || index >= rightPages.Length)
            return;

        for (int i = 0; i < rightPages.Length; i++)
        {
            rightPages[i].SetActive(i == index);
        }

        currentRightPage = index;
    }
}
