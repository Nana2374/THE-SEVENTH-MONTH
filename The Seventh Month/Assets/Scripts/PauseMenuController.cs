using UnityEngine;
using UnityEngine.UI;  // For UI components like Button and Slider
using UnityEngine.SceneManagement;  // For scene loading, if needed

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;  // The Pause Menu UI to be activated
    public Slider audioSlider;      // Audio Slider to control volume

    public AudioClip pauseMenuSound; // Sound to play when the pause menu opens and closes
    private AudioSource audioSource; // AudioSource to play the sound

    private bool isPaused = false;  // Track if the game is paused

    void Start()
    {
        // Initially hide the pause menu
        pauseMenuUI.SetActive(false);

        // Initialize the audio slider with the current volume
        audioSlider.value = AudioListener.volume;

        // Add listener for the audio slider
        audioSlider.onValueChanged.AddListener(SetVolume);

        // Get the AudioSource component attached to the GameObject
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check for the escape key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // Function to pause the game
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Stop the game time
        pauseMenuUI.SetActive(true);  // Show the pause menu

        // Play the pause menu sound (same sound for opening)
        if (audioSource != null && pauseMenuSound != null)
        {
            audioSource.PlayOneShot(pauseMenuSound);
        }
    }

    // Function to resume the game
    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;  // Resume the game time
        pauseMenuUI.SetActive(false);  // Hide the pause menu

        // Play the pause menu sound (same sound for closing)
        if (audioSource != null && pauseMenuSound != null)
        {
            audioSource.PlayOneShot(pauseMenuSound);
        }
    }

    // Function to adjust the audio volume from the slider
    void SetVolume(float volume)
    {
        AudioListener.volume = volume;  // Set the global volume
    }
}
