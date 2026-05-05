using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CarController carPrefab;
    [SerializeField] private LapTimer lapTimer;    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        carPrefab.rb.linearVelocity = Vector3.zero;
        carPrefab.rb.angularVelocity = Vector3.zero;
        carPrefab.rb.position = spawnPoint.position;
        carPrefab.rb.rotation = spawnPoint.rotation;
        carPrefab.transform.position = spawnPoint.position;
        carPrefab.transform.rotation = spawnPoint.rotation;
        carPrefab.rb.Sleep();
        
        lapTimer.ResetRace();       
    }
}
