using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class LapTimer : MonoBehaviour
{
    public int totalLaps = 3;
    public TMP_Text infoText;

    private int currentLap = 0;
    private float lapStartTime;
    private float raceStartTime;
    private float totalRaceTime;

    private bool raceStarted = false;
    private float currentLapTime;

    private List<float> lapTimes = new List<float>(); // List to store all lap times
    
    
    public UnityEvent<float> onRaceEnded;

    private void Awake()
    {
        ResetRace();
    }

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
            // Start race
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

        lapTimes.Add(lapTime); // Add the completed lap time to the list

        currentLap++;

        if (currentLap > totalLaps)
        {
            raceStarted = false;
            totalRaceTime = Time.time - raceStartTime; // Finalize total race time
            Debug.Log("Race finished!");
            Debug.Log($"Total Race Time: {FormatTime(totalRaceTime)}");

            Debug.Log("Lap Times:");
            foreach (var time in lapTimes)
            {
                Debug.Log(FormatTime(time));
            }

            onRaceEnded.Invoke(totalRaceTime);
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

                // Display all lap times
                for (int i = 0; i < lapTimes.Count; i++)
                {
                    text += $"Lap {i + 1}: {FormatTime(lapTimes[i])}\n";
                }
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
            text += $"Total Time: {FormatTime(currentTotalRaceTime)}\n";

            // Display previous lap times dynamically
            for (int i = 0; i < lapTimes.Count; i++)
            {
                text += $"Lap {i + 1}: {FormatTime(lapTimes[i])}\n";
            }
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

    // Add this reset method in the LapTimer class
    public void ResetRace()
    {
        // Reset all lap-related variables
        currentLap = 0;
        lapStartTime = 0f;
        raceStartTime = 0f;
        totalRaceTime = 0f;
        raceStarted = false;
        currentLapTime = 0f;

        // Clear lap times list
        lapTimes.Clear();

        // Update the info text to the initial state
        UpdateText(0f);

        Debug.Log("Race has been reset.");
    }
}
