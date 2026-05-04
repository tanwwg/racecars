using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    public LapTimer lapTimer;

    private void OnTriggerEnter(Collider other)
    {
        lapTimer.OnCrossStartLine();
    }
}