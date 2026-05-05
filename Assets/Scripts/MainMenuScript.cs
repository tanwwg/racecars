using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CarController carPrefab;
    [SerializeField] private LapTimer lapTimer;

    [SerializeField] private GameObject[] demoObjects;
    [SerializeField] private GameObject[] raceObjects;

    public bool isGameStarted = false;
    
    [SerializeField] private RaceLeaderboardManager leaderboardManager;

    public void StartGame()
    {
        if (isGameStarted) return;
        isGameStarted = true;
        
        foreach (var obj in demoObjects) obj.SetActive(false);
        foreach(var obj in raceObjects) obj.SetActive(true);
        
        carPrefab.rb.linearVelocity = Vector3.zero;
        carPrefab.rb.angularVelocity = Vector3.zero;
        carPrefab.rb.position = spawnPoint.position;
        carPrefab.rb.rotation = spawnPoint.rotation;
        carPrefab.transform.position = spawnPoint.position;
        carPrefab.transform.rotation = spawnPoint.rotation;
        carPrefab.rb.Sleep();
        
        lapTimer.ResetRace();       
    }

    public void OnRaceEnded(float time)
    {
        leaderboardManager.SubmitRaceTime(time);
        isGameStarted = false;
    }
}
