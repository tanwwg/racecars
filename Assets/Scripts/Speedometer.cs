using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private TMP_Text speedText;

    [Header("Display")]
    [SerializeField] private int decimalPlaces = 0;
    
    private Vector3? _lastPosition;

    float Distance(Vector3 pos1, Vector3 pos2)
    {
        var diff = pos1 - pos2;
        var distSq = (diff.x * diff.x) + (diff.y * diff.y) + (diff.z * diff.z);
        return Mathf.Sqrt(distSq);
    }

    float ToKmh(float mps)
    {
        return mps * 3.6f;
    }

    private void LateUpdate()
    {
        if (!_lastPosition.HasValue)
        {
            _lastPosition = target.position;
            return;
        }

        float distance = Distance(target.position, _lastPosition.Value);
        float speedMetersPerSecond = distance / Time.deltaTime;
        float kmh = ToKmh(speedMetersPerSecond);

        speedText.text = $"{kmh.ToString($"F{decimalPlaces}")} km/h";

        _lastPosition = target.position;
    }
}