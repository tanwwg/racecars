using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private TMP_Text speedText;

    [Header("Display")]
    [SerializeField] private bool useKmh = true;
    [SerializeField] private int decimalPlaces = 0;

    private Vector3 _lastPosition;
    private bool _hasLastPosition;

    private void LateUpdate()
    {
        if (target == null || speedText == null)
            return;

        if (!_hasLastPosition)
        {
            _lastPosition = target.position;
            _hasLastPosition = true;
            return;
        }

        float distance = Vector3.Distance(target.position, _lastPosition);
        float speedMetersPerSecond = distance / Time.deltaTime;

        float displaySpeed = useKmh
            ? speedMetersPerSecond * 3.6f
            : speedMetersPerSecond;

        string unit = useKmh ? "km/h" : "m/s";

        speedText.text = $"{displaySpeed.ToString($"F{decimalPlaces}")} {unit}";

        _lastPosition = target.position;
    }
}