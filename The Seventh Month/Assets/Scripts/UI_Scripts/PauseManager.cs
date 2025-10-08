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

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Add button listeners
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        // Optional: Allow ESC key to toggle pause
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
        Time.timeScale = 0f; // Freeze game time
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume game time
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Debug.Log("Game Resumed");
    }

    public void GoToMainMenu()
    {
        // Resume time before switching scenes
        Time.timeScale = 1f;

        // Replace "MainMenu" with the actual name of your main menu scene
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Returning to Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();

#if UNITY_EDITOR
        // If testing in the Unity Editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
