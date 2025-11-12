using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    public float scrollSpeed = 40f;
    public float endDelay = 2f;
    public RectTransform rectTransform;
    public float endYPosition = 1200f;

    void Start()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        if (rectTransform.anchoredPosition.y >= endYPosition)
        {
            StartCoroutine(ReturnToMainMenuAfterDelay());
            enabled = false; // stop scrolling
        }
    }

    private IEnumerator ReturnToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(endDelay);
        SceneManager.LoadScene("MainMenu"); // loads main menu directly
    }
}


