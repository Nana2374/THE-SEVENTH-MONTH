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
    public Button memoButton;
    public GameObject memoPage;
}

[System.Serializable]
public class PageFoldedMemos
{
    public FoldedMemoData[] memos;
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
    public AudioClip flipSound;

    [Header("Progressive Unlocking")]
    public DayUnlockData[] dayUnlocks;

    [Header("Folded Memos per Page")]
    public PageFoldedMemos[] pageFoldedMemos;

    private int currentRightPage = 0;
    private bool[] pageUnlocked;

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

        int savedDay = PlayerPrefs.GetInt("SavedDay", 1);
        InitializeFolder(savedDay);
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
        foreach (var page in pageFoldedMemos)
        {
            foreach (var memo in page.memos)
            {
                if (memo.memoPage != memoPage)
                    memo.memoPage.SetActive(false);
            }
        }

        memoPage.SetActive(!memoPage.activeSelf);
        PlaySound(flipSound);
    }

    private void HideAllPagesAndTabs()
    {
        foreach (var page in rightPages)
            if (page != null) page.SetActive(false);

        foreach (var tab in tabButtons)
            if (tab != null) tab.gameObject.SetActive(false);
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

    // ==============================
    // 📖 PAGE MANAGEMENT SYSTEM
    // ==============================

    public void InitializeFolder(int currentDay)
    {
        pageUnlocked = new bool[rightPages.Length];
        HideAllPagesAndTabs();
        UpdateUnlockedPages(currentDay);
        ShowFirstUnlockedPage();
    }

    public void UpdateUnlockedPages(int currentDay)
    {
        for (int i = 0; i < pageUnlocked.Length; i++)
            pageUnlocked[i] = false;

        foreach (var data in dayUnlocks)
        {
            if (data.dayNumber <= currentDay)
            {
                // Unlock pages
                foreach (var page in data.pagesToUnlock)
                {
                    if (page != null)
                    {
                        for (int i = 0; i < rightPages.Length; i++)
                        {
                            if (rightPages[i] == page)
                            {
                                pageUnlocked[i] = true;
                                break;
                            }
                        }
                    }
                }

                // Unlock tabs
                foreach (var tab in data.tabsToUnlock)
                {
                    if (tab != null)
                        tab.gameObject.SetActive(true);
                }
            }
        }

        Debug.Log($"[FolderManager] Unlocked up to Day {currentDay}");
    }

    private void ShowFirstUnlockedPage()
    {
        for (int i = 0; i < rightPages.Length; i++)
        {
            if (pageUnlocked[i])
            {
                ShowRightPage(i, false);
                return;
            }
        }

        Debug.LogWarning("[FolderManager] No unlocked pages found!");
    }

    public void ShowRightPage(int index, bool playSound = true)
    {
        if (index < 0 || index >= rightPages.Length)
            return;
        if (!pageUnlocked[index])
        {
            Debug.Log("[FolderManager] Tried to access locked page index " + index);
            return;
        }

        if (playSound)
            PlaySound(flipSound);

        for (int i = 0; i < rightPages.Length; i++)
        {
            if (rightPages[i] != null)
                rightPages[i].SetActive(i == index);
        }

        currentRightPage = index;
        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        bool hasNext = false;
        for (int i = currentRightPage + 1; i < pageUnlocked.Length; i++)
        {
            if (pageUnlocked[i]) { hasNext = true; break; }
        }

        bool hasPrev = false;
        for (int i = currentRightPage - 1; i >= 0; i--)
        {
            if (pageUnlocked[i]) { hasPrev = true; break; }
        }

        if (nextPageButton != null) nextPageButton.interactable = hasNext;
        if (prevPageButton != null) prevPageButton.interactable = hasPrev;
    }

    public void NextPage()
    {
        int nextIndex = currentRightPage + 1;
        while (nextIndex < rightPages.Length && !pageUnlocked[nextIndex])
            nextIndex++;

        if (nextIndex < rightPages.Length)
            ShowRightPage(nextIndex);
        else
            Debug.Log("[FolderManager] No further unlocked page to the right.");
    }

    public void PreviousPage()
    {
        int prevIndex = currentRightPage - 1;
        while (prevIndex >= 0 && !pageUnlocked[prevIndex])
            prevIndex--;

        if (prevIndex >= 0)
            ShowRightPage(prevIndex);
        else
            Debug.Log("[FolderManager] No further unlocked page to the left.");
    }

    public bool IsCaseUnlocked(CustomerCase customerCase)
    {
        int currentDay = PlayerPrefs.GetInt("SavedDay", 1);
        return customerCase.unlockDay <= currentDay;
    }
}
