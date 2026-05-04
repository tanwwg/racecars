using UnityEngine;
using TMPro;

public class LapTimer : MonoBehaviour
{
    public int totalLaps = 3;
    public TMP_Text infoText;

    private int currentLap = 0;
    private float lapStartTime;
    private float bestLapTime = Mathf.Infinity;
    private float raceStartTime;
    private float totalRaceTime;

    private bool raceStarted = false;
    private float currentLapTime;

    void Update()
    {
        if (!raceStarted)
        {
            UpdateText(0f);
            return;
        }

        currentLapTime = Time.time - lapStartTime;
        UpdateText(currentLapTime);
    }

    public void OnCrossStartLine()
    {
        if (!raceStarted)
        {
            // start race
            raceStarted = true;
            currentLap = 1;
            lapStartTime = Time.time;
            raceStartTime = Time.time; // Track when the race started
            return;
        }

        CompleteLap();
    }

    void CompleteLap()
    {
        float lapTime = Time.time - lapStartTime;

        if (lapTime < bestLapTime)
            bestLapTime = lapTime;

        currentLap++;

        if (currentLap > totalLaps)
        {
            raceStarted = false;
            totalRaceTime = Time.time - raceStartTime; // Finalize total race time
            Debug.Log("Race finished!");
            Debug.Log($"Total Race Time: {FormatTime(totalRaceTime)}");
            return;
        }

        lapStartTime = Time.time;
    }

    void UpdateText(float lapTime)
    {
        string text = "";

        // Always calculate the total race time dynamically
        float currentTotalRaceTime = raceStarted ? Time.time - raceStartTime : totalRaceTime;

        if (!raceStarted)
        {
            if (currentLap > totalLaps)
            {
                // Display final times after the race ends
                text += "Race Finished!\n";
                text += $"Total Time: {FormatTime(currentTotalRaceTime)}\n";
                text += $"Best Lap: {FormatTime(bestLapTime)}\n";
            }
            else
            {
                text += "Ready\n";
                text += "Cross line to start\n";
                text += $"Total Time: {FormatTime(currentTotalRaceTime)}\n"; // Display total time even before race starts
            }
        }
        else
        {
            text += $"Lap: {currentLap}/{totalLaps}\n";
            text += $"Time: {FormatTime(lapTime)}\n";
            text += $"Total Time: {FormatTime(currentTotalRaceTime)}\n"; // Always display the current total race time

            if (bestLapTime < Mathf.Infinity)
                text += $"Best: {FormatTime(bestLapTime)}\n";
            else
                text += $"Best: --:--.---\n";
        }

        infoText.text = text;
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}