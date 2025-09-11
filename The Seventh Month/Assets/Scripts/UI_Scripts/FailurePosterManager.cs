using UnityEngine;
using UnityEngine.UI;

public class FailurePosterManager : MonoBehaviour
{
    public Image posterImage; // Assign the UI Image of the bulletin board
    public GameObject posterContainer; // Parent GameObject for enabling/disabling the poster

    // Show a poster sprite
    public void ShowPoster(Sprite posterSprite)
    {
        if (posterImage != null && posterContainer != null)
        {
            posterImage.sprite = posterSprite;
            posterContainer.SetActive(true);
        }
    }

    // Hide the poster
    public void HidePoster()
    {
        if (posterContainer != null)
        {
            posterContainer.SetActive(false);
        }
    }
}
