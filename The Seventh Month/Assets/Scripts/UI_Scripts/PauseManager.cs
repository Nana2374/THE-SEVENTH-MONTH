using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;  // The Pause Menu Panel
    public Button resumeButton;     // Resume button
    public Button mainMenuButton;   // "To Main Menu" button
    public Button quitButton;       // "Quit Game" button

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioSource audioSource;

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Add button listeners with sound
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => { PlayClickSound(); ResumeGame(); });

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => { PlayClickSound(); GoToMainMenu(); });

        if (quitButton != null)
            quitButton.onClick.AddListener(() => { PlayClickSound(); QuitGame(); });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        Debug.Log("Game Resumed");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Returning to Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}

