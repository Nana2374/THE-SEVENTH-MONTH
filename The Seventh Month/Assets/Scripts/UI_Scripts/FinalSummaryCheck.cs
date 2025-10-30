using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FinalSummaryUI : MonoBehaviour
{
    public TextMeshProUGUI doomedCountText;
    public TextMeshProUGUI paycheckText;

    public void ShowSummary(int doomed)
    {
        gameObject.SetActive(true);

        float basePay = 240f;            // Fixed base pay
        float penalty = doomed * 21f;     // $21 penalty per doomed customer
        float finalPay = Mathf.Max(0f, basePay - penalty);  // Prevent negative pay

        doomedCountText.text = $"{doomed}";
        paycheckText.text = $"{finalPay:F2}";
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // adjust scene name
    }
}
