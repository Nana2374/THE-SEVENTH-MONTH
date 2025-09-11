using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FailurePosterManager : MonoBehaviour
{
    [Header("Poster Settings")]
    public GameObject posterPrefab;   // Prefab with Image component
    public RectTransform posterBoard; // The UI panel area (bulletin board)
    public float posterDelay = 1.5f;  // Time before poster appears
    public Vector2 padding = new Vector2(50, 50); // Keep posters inside edges

    // Call this from SolutionChecker when a case fails
    public void QueuePoster(Sprite posterSprite)
    {
        if (posterSprite != null)
        {
            StartCoroutine(SpawnPosterAfterDelay(posterSprite));
        }
    }

    private IEnumerator SpawnPosterAfterDelay(Sprite posterSprite)
    {
        yield return new WaitForSeconds(posterDelay);

        // Spawn poster under the board
        GameObject newPoster = Instantiate(posterPrefab, posterBoard);
        RectTransform rt = newPoster.GetComponent<RectTransform>();
        Image img = newPoster.GetComponent<Image>();

        if (img != null)
        {
            img.sprite = posterSprite;
        }

        // Random position within the posterBoard area
        if (posterBoard != null && rt != null)
        {
            float x = Random.Range(padding.x, posterBoard.rect.width - padding.x);
            float y = Random.Range(padding.y, posterBoard.rect.height - padding.y);

            // Convert local position
            rt.anchoredPosition = new Vector2(x - posterBoard.rect.width / 2,
                                              y - posterBoard.rect.height / 2);

            // Optional: random tilt for realism
            float randomRotation = Random.Range(-15f, 15f);
            rt.localRotation = Quaternion.Euler(0, 0, randomRotation);
        }
    }
}
