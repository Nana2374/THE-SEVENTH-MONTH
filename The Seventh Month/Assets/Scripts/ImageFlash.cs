using UnityEngine;
using UnityEngine.UI;

public class ImageFlicker : MonoBehaviour
{
    [Header("Images to Flicker")]
    public Image[] images;  // assign your images here

    [Header("Flicker Settings")]
    public float duration = 1f;             // total flicker time for this fail
    public float flickerIntervalMin = 0.05f; // min time between on/off
    public float flickerIntervalMax = 0.15f; // max time between on/off

    private bool isFlickering = false;
    private float flickerTimer = 0f;
    private float durationTimer = 0f;
    private Image currentImage = null;
    private bool isImageOn = false;

    void Start()
    {
        // Hide all images initially
        foreach (var img in images)
        {
            if (img != null)
                img.enabled = false;
        }
    }

    void Update()
    {
        if (!isFlickering) return;

        durationTimer -= Time.deltaTime;
        flickerTimer -= Time.deltaTime;

        if (flickerTimer <= 0f && currentImage != null)
        {
            // Toggle the current image on/off
            isImageOn = !isImageOn;
            currentImage.enabled = isImageOn;

            // Reset flicker timer
            flickerTimer = Random.Range(flickerIntervalMin, flickerIntervalMax);
        }

        // Stop flickering after duration
        if (durationTimer <= 0f)
        {
            isFlickering = false;
            if (currentImage != null)
                currentImage.enabled = false;
        }
    }

    // Call this to start a failure flicker
    public void TriggerFlicker()
    {
        if (images.Length == 0) return;

        // Choose a new random image each fail
        currentImage = images[Random.Range(0, images.Length)];
        isFlickering = true;
        durationTimer = duration;
        flickerTimer = 0f; // start flickering immediately
        isImageOn = false; // will turn on in the first update
    }
}