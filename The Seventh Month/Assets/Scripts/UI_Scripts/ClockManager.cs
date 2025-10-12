using UnityEngine;
using TMPro;

public class ClockManager : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    private int currentHour = 13;


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
        }
    }


    private void UpdateClockUI()
    {
        clockText.text = $"{currentHour:00}:00";

    }
}

