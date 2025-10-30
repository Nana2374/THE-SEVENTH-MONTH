using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class FinalSummaryUI : MonoBehaviour
{
    public TextMeshProUGUI doomedCountText;
    public TextMeshProUGUI paycheckText;
    public TextMeshProUGUI paycheckInWordsText; // new text field to show words

    public void ShowSummary(int doomed)
    {
        gameObject.SetActive(true);

        float basePay = 240f;            // Fixed base pay
        float penalty = doomed * 21f;     // $21 penalty per doomed customer
        float finalPay = Mathf.Max(0f, basePay - penalty);  // Prevent negative pay

        doomedCountText.text = $"Customers Doomed: {doomed}";
        paycheckText.text = $"Final Paycheck: ${finalPay:F2}";
        paycheckInWordsText.text = NumberToWords((int)finalPay); // show spelled out
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // adjust scene name
    }

    // Helper function to convert numbers to words
    private string NumberToWords(int number)
    {
        if (number == 0)
            return "ZERO";

        if (number < 0)
            return "MINUS " + NumberToWords(Math.Abs(number));

        string words = "";

        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " HUNDRED ";
            number %= 100;
        }

        if (number > 0)
        {
            string[] unitsMap = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN",
                                  "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
            string[] tensMap = { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
        }

        return words.Trim();
    }
}
