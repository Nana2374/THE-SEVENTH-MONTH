using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource; // Drag your Audio Source here in the Inspector

    void Start()
    {
        // Get the Button component if it is on the same GameObject
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}

