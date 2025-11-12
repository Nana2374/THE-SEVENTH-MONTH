using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public float scrollSpeed = 40f;
    public float endDelay = 2f; // wait time after credits finish
    public RectTransform rectTransform;
    public float endYPosition = 1200f; // Adjust this based on how tall your credits are

    void Start()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Move the credits upward
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        // Check if the credits have reached the target end position
        if (rectTransform.anchoredPosition.y >= endYPosition)
        {
            StartCoroutine(ReturnToMainMenuAfterDelay());
            enabled = false; // stop further scrolling
        }
    }

    private IEnumerator ReturnToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(endDelay);
        SceneManager.LoadScene("MainMenu"); // make sure this matches your main menu scene name
    }
}
