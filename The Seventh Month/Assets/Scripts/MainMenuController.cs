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
        if (PlayerPrefs.HasKey("SavedDay"))
        {
            int savedDay = PlayerPrefs.GetInt("SavedDay");
            continueButton.gameObject.SetActive(true);

            if (continueText != null)
                continueText.text = $"(Day {savedDay})";
        }
        else
        {
            continueButton.gameObject.SetActive(false);
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
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(0f);

        // Load the loading scene asynchronously
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("Loading");

        // Wait until the loading scene is fully loaded
        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
