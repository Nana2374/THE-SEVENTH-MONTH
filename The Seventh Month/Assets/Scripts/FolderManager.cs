using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DayUnlockData
{
    public int dayNumber;
    public GameObject[] pagesToUnlock;
    public Button[] tabsToUnlock;
}

[System.Serializable]
public class FoldedMemoData
{
    public Button memoButton;      // The small folded memo button
    public GameObject memoPage;    // The page it opens
}

[System.Serializable]
public class PageFoldedMemos
{
    public FoldedMemoData[] memos; // Array of memos for this page
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

    [Header("Audio Clips")]
    public AudioSource audioSource;
    public AudioClip folderOpenSound;
    public AudioClip folderCloseSound;
    public AudioClip flipSound; // page flips / memo toggles

    [Header("Progressive Unlocking")]
    public DayUnlockData[] dayUnlocks;

    [Header("Folded Memos per Page")]
    public PageFoldedMemos[] pageFoldedMemos; // Array of pages, each containing memos

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

        // 🔹 Load saved day, or default to 1 if none saved
        int savedDay = PlayerPrefs.GetInt("SavedDay", 1); // match CustomerManager key


        // 🔓 Hide all pages/tabs first, then initialize for Day 1
        InitializeFolder(savedDay); // <-- this ensures Day 1 pages/tabs are unlocked on the last day saved
        InitializeFoldedMemos();
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }


    private void InitializeFoldedMemos()
    {
        for (int pageIndex = 0; pageIndex < pageFoldedMemos.Length; pageIndex++)
        {
            var memos = pageFoldedMemos[pageIndex].memos;
            if (memos == null) continue;

            foreach (var memo in memos)
            {
                if (memo.memoButton != null && memo.memoPage != null)
                {
                    memo.memoPage.SetActive(false);
                    memo.memoButton.onClick.AddListener(() =>
                    {
                        ToggleFoldedMemo(memo.memoPage);
                    });
                }
            }
        }
    }

    private void ToggleFoldedMemo(GameObject memoPage)
    {
        // Hide all other memos of all pages
        foreach (var page in pageFoldedMemos)
        {
            foreach (var memo in page.memos)
            {
                if (memo.memoPage != memoPage)
                    memo.memoPage.SetActive(false);
            }
        }

        // Toggle the clicked memo page
        memoPage.SetActive(!memoPage.activeSelf);
        PlaySound(flipSound);
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

        PlaySound(folderOpenSound);

        if (useScaleAnimation)
            LeanTween.scale(expandedFolder, openScale, openDuration).setEaseOutBack();
        else
            expandedFolder.transform.localScale = openScale;
    }

    public void CloseFolder()
    {
        PlaySound(folderCloseSound);

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


    public void ShowRightPage(int index, bool playSound = true)
    {
        if (index < 0 || index >= rightPages.Length) return;

        if (playSound)
            PlaySound(flipSound);

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
        for (int i = 0; i < rightPages.Length; i++)
        {
            if (rightPages[i] != null && rightPages[i].activeSelf)
            {
                ShowRightPage(i, false); // <-- no sound on startup
                return;
            }
        }
    }

}
