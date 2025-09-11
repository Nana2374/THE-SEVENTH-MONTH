using UnityEngine;
using TMPro;

public class ClockManager : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    public int currentDay = 1; // Day 1 by default
    private int currentHour = 13; // Start at 1 PM

    public delegate void DayEndedHandler();
    public event DayEndedHandler OnDayEnded;

    public void ResetClock()
    {
        currentHour = 13;
        UpdateClockUI();
    }

    public void AdvanceHour()
    {
        if (currentHour < 18)
        {
            currentHour++;
            UpdateClockUI();

            if (currentHour >= 18)
            {
                EndDay();
            }
        }
    }

    private void EndDay()
    {
        Debug.Log($"Day {currentDay} ended!");
        OnDayEnded?.Invoke(); // notify listeners (CustomerManager)
    }

    public void StartNextDay()
    {
        currentDay++;
        currentHour = 13;
        UpdateClockUI();
    }

    private void UpdateClockUI()
    {
        int displayHour = currentHour > 12 ? currentHour - 12 : currentHour;
        string suffix = currentHour >= 12 ? "PM" : "AM";
        clockText.text = $"Day {currentDay} - {displayHour}:00 {suffix}";
    }
}
