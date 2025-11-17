using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button continueButton;
    public TMP_Text continueText; // optional: show "Continue Day X"

    [Header("Confirmation Popup")]
    public GameObject newGamePopup;  // The confirmation panel (set this in the Inspector)
    public Button yesButton;         // "Yes" button on popup
    public Button noButton;          // "No" button on popup

    private void Start()
    {
        CheckSavedProgress();

        // Ensure popup is hidden at start
        if (newGamePopup != null)
            newGamePopup.SetActive(false);

        // Hook up popup buttons
        if (yesButton != null)
            yesButton.onClick.AddListener(OnConfirmNewGame);

        if (noButton != null)
            noButton.onClick.AddListener(OnCancelNewGame);
    }

    private void CheckSavedProgress()
    {
        // Always show continue button
        continueButton.gameObject.SetActive(true);

        if (PlayerPrefs.HasKey("SavedDay"))
        {
            int savedDay = PlayerPrefs.GetInt("SavedDay");

            continueButton.interactable = true;

            if (continueText != null)
            {
                continueText.text = $"(Day {savedDay})";
                continueText.color = Color.white; // normal text color
            }
        }
        else
        {
            // Grey out if no save
            continueButton.interactable = false;

            if (continueText != null)
            {
                continueText.text = "(No Save)";
                continueText.color = new Color(1, 1, 1, 0.4f); // dimmed text
            }
        }
    }

    public void StartGame()
    {
        if (newGamePopup != null)
            newGamePopup.SetActive(true);
    }

    // Player clicks "Yes" → start new game
    private void OnConfirmNewGame()
    {
        if (newGamePopup != null)
            newGamePopup.SetActive(false);

        // Delete ALL PlayerPrefs keys to fully reset
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("[MainMenu] Cleared all save data — starting fresh game.");

        StartCoroutine(StartGameCoroutine());
    }

    // Player clicks "No" → close popup, stay on menu
    private void OnCancelNewGame()
    {
        if (newGamePopup != null)
            newGamePopup.SetActive(false);
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("SavedDay"))
        {
            int savedDay = PlayerPrefs.GetInt("SavedDay");
            Debug.Log($"[MainMenu] Continuing game from Day {savedDay}");

            SceneManager.LoadScene("Lvl1");
        }
        else
        {
            Debug.Log("[MainMenu] No saved progress found. Starting tutorial instead.");
            SceneManager.LoadScene("Instructions");
        }
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return null;

        SceneManager.LoadScene("Instructions");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

