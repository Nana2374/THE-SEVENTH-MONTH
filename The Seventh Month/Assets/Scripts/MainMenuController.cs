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


    private void Start()
    {
        CheckSavedProgress();
    }

    private void CheckSavedProgress()
    {
        if (PlayerPrefs.HasKey("SavedDay"))
        {
            int savedDay = PlayerPrefs.GetInt("SavedDay");
            continueButton.gameObject.SetActive(true);

            if (continueText != null)
                continueText.text = $"Continue (Day {savedDay})";
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }
    }


    public void StartGame()
    {
        // Reset save data for new game
        PlayerPrefs.DeleteKey("SavedDay");
        StartCoroutine(StartGameCoroutine());
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
