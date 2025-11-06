using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DayUnlockData
{
    public int dayNumber;
    public GameObject[] pagesToUnlock;
    public Button[] tabsToUnlock;
}

public class FolderManager : MonoBehaviour
{
    [Header("Folder Elements")]
    public Button folderButton;
    public GameObject expandedFolder;
    public Button backgroundButton;

    [Header("Pages")]
    public GameObject[] rightPages;

    [Header("Navigation")]
    public Button nextPageButton;
    public Button prevPageButton;
    public Button[] tabButtons;

    [Header("Animation Settings")]
    public bool useScaleAnimation = true;
    public float openDuration = 0.3f;
    public float closeDuration = 0.2f;
    public Vector3 foldedScale = Vector3.one;
    public Vector3 openScale = Vector3.one;

    [Header("Progressive Unlocking")]
    public DayUnlockData[] dayUnlocks;

    private int currentRightPage = 0;

    void Start()
    {
        // Start closed
        expandedFolder.SetActive(false);
        backgroundButton.gameObject.SetActive(false);
        expandedFolder.transform.localScale = foldedScale;

        // Setup listeners
        folderButton.onClick.AddListener(OpenFolder);
        backgroundButton.onClick.AddListener(CloseFolder);

        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NextPage);
        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(PreviousPage);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => ShowRightPage(index));
        }

        // 🔓 Hide all pages/tabs first, then initialize for Day 1
        InitializeFolder(1); // <-- this ensures Day 1 pages/tabs are unlocked immediately
    }

    private void HideAllPagesAndTabs()
    {
        foreach (var page in rightPages)
            if (page != null) page.SetActive(false);

        foreach (var tab in tabButtons)
            if (tab != null) tab.gameObject.SetActive(false); // hide completely
    }

    public void OpenFolder()
    {
        expandedFolder.SetActive(true);
        backgroundButton.gameObject.SetActive(true);

        if (useScaleAnimation)
            LeanTween.scale(expandedFolder, openScale, openDuration).setEaseOutBack();
        else
            expandedFolder.transform.localScale = openScale;
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
        if (index < 0 || index >= rightPages.Length) return;

        for (int i = 0; i < rightPages.Length; i++)
        {
            if (rightPages[i] != null)
                rightPages[i].SetActive(i == index);
        }

        currentRightPage = index;

        if (nextPageButton != null)
            nextPageButton.interactable = (currentRightPage < rightPages.Length - 1);
        if (prevPageButton != null)
            prevPageButton.interactable = (currentRightPage > 0);
    }

    public void NextPage() => ShowRightPage(currentRightPage + 1);

    public void PreviousPage() => ShowRightPage(currentRightPage - 1);

    // 🔓 Build-up system: keeps previous days unlocked
    public void UpdateUnlockedPages(int currentDay)
    {
        // Unlock all pages/tabs up to the current day
        foreach (var data in dayUnlocks)
        {
            if (data.dayNumber <= currentDay)
            {
                // Unlock pages
                foreach (var page in data.pagesToUnlock)
                {
                    if (page != null)
                        page.SetActive(true);
                }

                // Unlock tabs
                foreach (var tab in data.tabsToUnlock)
                {
                    if (tab != null)
                        tab.gameObject.SetActive(true); // show unlocked tab
                }
            }
        }

        Debug.Log($"[FolderManager] Up to Day {currentDay}: All previous pages/tabs unlocked.");
    }

    public void InitializeFolder(int currentDay)
    {
        HideAllPagesAndTabs(); // hide everything first
        UpdateUnlockedPages(currentDay); // unlock pages/tabs up to saved day
        ShowFirstUnlockedPage(); // show the first unlocked page
    }

    private void ShowFirstUnlockedPage()
    {
        // Find the first page that’s active and show it
        for (int i = 0; i < rightPages.Length; i++)
        {
            if (rightPages[i] != null && rightPages[i].activeSelf)
            {
                ShowRightPage(i);
                return;
            }
        }
    }
}
