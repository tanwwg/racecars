using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class RaceLeaderboardManager : MonoBehaviour
{
    [SerializeField] private string leaderboardId;
    [SerializeField] private TMP_Text leaderboardText;
    [SerializeField, Min(1)] private int leaderboardLimit = 10;

    private Task initializationTask;
    private bool isRefreshing;
    private bool isSubmitting;

    private string loggedInPlayerName; // Store the logged-in player's name

    private async void Start()
    {
        try
        {
            await RefreshLeaderboardAsync();
        }
        catch (Exception e)
        {
            SetLeaderboardText(e.Message);
        }
    }

    public async void SubmitRaceTime(float totalRaceTimeSeconds)
    {
        try
        {
            await SubmitRaceTimeAsync(totalRaceTimeSeconds);
        }
        catch (Exception e)
        {
            SetLeaderboardText(e.Message);
        }
    }

    public async Task SubmitRaceTimeAsync(float totalRaceTimeSeconds)
    {
        if (isSubmitting)
        {
            return;
        }

        if (!HasLeaderboardId())
        {
            return;
        }

        isSubmitting = true;
        SetLeaderboardText("Submitting race time...");

        try
        {
            await EnsureInitializedAsync();
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, totalRaceTimeSeconds);
            await RefreshLeaderboardAsync();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SetLeaderboardText("Could not submit leaderboard score.");
        }
        finally
        {
            isSubmitting = false;
        }
    }

    public async Task RefreshLeaderboardAsync()
    {
        if (isRefreshing)
        {
            return;
        }

        if (!HasLeaderboardId())
        {
            return;
        }

        isRefreshing = true;
        SetLeaderboardText("Loading leaderboard...");

        try
        {
            await EnsureInitializedAsync();

            LeaderboardScoresPage scores = await LeaderboardsService.Instance.GetScoresAsync(
                leaderboardId,
                new GetScoresOptions { Offset = 0, Limit = leaderboardLimit }
            );

            DisplayScores(scores);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            SetLeaderboardText("Could not load leaderboard.");
        }
        finally
        {
            isRefreshing = false;
        }
    }

    private Task EnsureInitializedAsync()
    {
        if (initializationTask == null)
        {
            initializationTask = InitializeServicesAsync();
        }

        return initializationTask;
    }

    private async Task InitializeServicesAsync()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Fetch the logged-in player's name after signing in
        loggedInPlayerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        if (string.IsNullOrWhiteSpace(loggedInPlayerName))
        {
            loggedInPlayerName = "You"; // Default to "You" if no name is available
        }
    }

    private bool HasLeaderboardId()
    {
        if (!string.IsNullOrWhiteSpace(leaderboardId))
        {
            return true;
        }

        SetLeaderboardText("Set Leaderboard ID in the Inspector.");
        Debug.LogWarning("RaceLeaderboardManager needs a leaderboard ID before it can use Unity Leaderboards.");
        return false;
    }

    private void DisplayScores(LeaderboardScoresPage scores)
    {
        if (scores == null || scores.Results == null || scores.Results.Count == 0)
        {
            SetLeaderboardText($"Your Player Name: {loggedInPlayerName}\nNo leaderboard times yet.");
            return;
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Top Times");

        // Display the logged-in player's name
        builder.AppendLine($"Your Player Name: {loggedInPlayerName}");

        foreach (LeaderboardEntry entry in scores.Results)
        {
            builder.Append(entry.Rank + 1);
            builder.Append(". ");
            builder.Append(GetDisplayName(entry));
            builder.Append(" - ");
            builder.AppendLine(FormatTime(entry.Score));
        }

        SetLeaderboardText(builder.ToString());
    }

    private static string GetDisplayName(LeaderboardEntry entry)
    {
        if (!string.IsNullOrWhiteSpace(entry.PlayerName))
        {
            return entry.PlayerName;
        }

        if (!string.IsNullOrWhiteSpace(entry.PlayerId))
        {
            return entry.PlayerId.Length > 8 ? entry.PlayerId.Substring(0, 8) : entry.PlayerId;
        }

        return "Player";
    }

    private static string FormatTime(double time)
    {
        int minutes = Mathf.FloorToInt((float)(time / 60d));
        int seconds = Mathf.FloorToInt((float)(time % 60d));
        int milliseconds = Mathf.FloorToInt((float)((time * 1000d) % 1000d));

        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    private void SetLeaderboardText(string text)
    {
        if (leaderboardText != null)
        {
            leaderboardText.text = text;
        }
    }
}
