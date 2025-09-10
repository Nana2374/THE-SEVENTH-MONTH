using UnityEngine;
using TMPro; // Only if you use TextMeshPro

public class ClockManager : MonoBehaviour
{
    public TextMeshProUGUI clockText; // Assign in inspector
    private int currentHour = 13; // Start at 1 PM (13:00)

    public void ResetClock()
    {
        currentHour = 13;
        UpdateClockUI();
    }

    public void AdvanceHour()
    {
        if (currentHour < 18) // 6 PM = 18:00
        {
            currentHour++;
            UpdateClockUI();
        }
    }

    private void UpdateClockUI()
    {
        int displayHour = currentHour > 12 ? currentHour - 12 : currentHour;
        string suffix = currentHour >= 12 ? "PM" : "AM";
        clockText.text = $"{displayHour}:00 {suffix}";
    }
}
