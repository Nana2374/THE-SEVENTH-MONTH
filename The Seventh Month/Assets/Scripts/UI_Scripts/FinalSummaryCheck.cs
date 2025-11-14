using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class FinalSummaryUI : MonoBehaviour
{
    public TextMeshProUGUI doomedCountText;
    public TextMeshProUGUI paycheckText;
    public TextMeshProUGUI paycheckInWordsText;

    public Button nextButton;

    public AudioSource audioSource;
    public AudioClip moneySound;

    private int totalFailures = 0; // Track failures

    void Start()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    public void ShowSummary(int doomed)
    {
        totalFailures = doomed; // store total failures
        gameObject.SetActive(true);
        audioSource.PlayOneShot(moneySound);

        float basePay = 240f;
        float penalty = doomed * 21f;
        float finalPay = Mathf.Max(0f, basePay - penalty);

        doomedCountText.text = $"{doomed}";
        paycheckText.text = $"{finalPay:F2}";
        paycheckInWordsText.text = NumberToWords((int)finalPay);
    }

    private void OnNextButtonClicked()
    {
        if (totalFailures == 0)
        {
            // Perfect playthrough → show end cutscene
            SceneManager.LoadScene("EndCutScene");
        }
        else
        {
            // Any failure → go straight to credits
            SceneManager.LoadScene("Credits");
        }
    }

    private string NumberToWords(int number)
    {
        if (number == 0) return "ZERO";
        if (number < 0) return "MINUS " + NumberToWords(Math.Abs(number));

        string words = "";

        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " HUNDRED ";
            number %= 100;
        }

        if (number > 0)
        {
            string[] unitsMap = { "ZERO","ONE","TWO","THREE","FOUR","FIVE","SIX","SEVEN","EIGHT","NINE","TEN",
                                  "ELEVEN","TWELVE","THIRTEEN","FOURTEEN","FIFTEEN","SIXTEEN","SEVENTEEN","EIGHTEEN","NINETEEN" };
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
