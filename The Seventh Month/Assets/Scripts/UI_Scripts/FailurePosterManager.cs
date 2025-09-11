using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FailurePosterManager : MonoBehaviour
{
    [Header("Poster Settings")]
    public GameObject posterPrefab;       // Prefab must have an Image component
    public RectTransform posterBoard;     // The UI panel (bulletin board)
    public float posterDelay = 1.5f;      // Time before poster appears
    public Vector2 padding = new Vector2(50, 50); // Keep posters inside edges

    // Call this from SolutionChecker or CustomerManager when a case fails
    public void QueuePoster(Sprite posterSprite)
    {
        if (posterSprite == null)
        {
            Debug.LogWarning("QueuePoster called with null sprite!");
            return;
        }

        StartCoroutine(SpawnPosterAfterDelay(posterSprite));
    }

    private IEnumerator SpawnPosterAfterDelay(Sprite posterSprite)
    {
        yield return new WaitForSeconds(posterDelay);

        if (posterPrefab == null || posterBoard == null)
        {
            Debug.LogError("PosterPrefab or PosterBoard is not assigned!");
            yield break;
        }

        // Instantiate poster as child of posterBoard
        GameObject newPoster = Instantiate(posterPrefab, posterBoard);
        newPoster.SetActive(true); // Ensure it’s active

        // Assign sprite
        Image img = newPoster.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("Poster prefab missing Image component!");
            yield break;
        }
        img.sprite = posterSprite;

        // Get RectTransform
        RectTransform rt = newPoster.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Poster prefab missing RectTransform!");
            yield break;
        }

        // Random position inside board with padding
        float x = Random.Range(padding.x, posterBoard.rect.width - padding.x);
        float y = Random.Range(padding.y, posterBoard.rect.height - padding.y);

        // Convert to anchored position (centered pivot)
        rt.anchoredPosition = new Vector2(x - posterBoard.rect.width / 2, y - posterBoard.rect.height / 2);

        // Optional: random rotation for realism
        float randomRotation = Random.Range(-15f, 15f);
        rt.localRotation = Quaternion.Euler(0, 0, randomRotation);

        Debug.Log($"Poster spawned at {rt.anchoredPosition} with rotation {randomRotation}");
    }
}
