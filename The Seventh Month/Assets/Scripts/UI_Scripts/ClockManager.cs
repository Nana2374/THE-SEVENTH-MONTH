using UnityEngine;
using TMPro; // Only if you use TextMeshPro

public class ClockManager : MonoBehaviour
{
    public TextMeshProUGUI clockText; // Assign in inspector
    private int currentHour = 13; // Start at 13:00

    public void ResetClock()
    {
        currentHour = 13;
        UpdateClockUI();
    }

    public void AdvanceHour()
    {
        if (currentHour < 18) // End at 18:00
        {
            currentHour++;
            UpdateClockUI();
        }
    }

    private void UpdateClockUI()
    {
        // Format as 24-hour time with leading zero if needed
        clockText.text = $"{currentHour:00}:00";
    }
}
