using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayTransitionManager : MonoBehaviour
{
    public CanvasGroup transitionPanel;   // black panel for fade
    public TextMeshProUGUI transitionText;
    public float fadeDuration = 1.5f;

    public IEnumerator PlayTransition(int day)
    {
        // Enable panel
        transitionPanel.gameObject.SetActive(true);
        transitionText.text = $"Day {day}";

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            transitionPanel.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        // Pause while panel is fully visible
        yield return new WaitForSeconds(1.5f);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            transitionPanel.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        // Hide panel
        transitionPanel.gameObject.SetActive(false);
    }
}
