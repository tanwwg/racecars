using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EngineSound : MonoBehaviour
{
    public Rigidbody rb;
    public float throttle;

    [Header("Speed")]
    public float maxSpeed = 40f; // m/s

    [Header("Audio")]
    public float minPitch = 0.7f;
    public float maxPitch = 2.0f;
    public float minVolume = 0.25f;
    public float maxVolume = 1.0f;

    [Header("Smoothing")]
    public float pitchSmooth = 5f;
    public float volumeSmooth = 5f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }

    void Start()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
    
    public void SetThrottle(float t) => throttle = t;

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;
        float t = Mathf.Clamp01(speed / maxSpeed);
        
        float throttleRev = Mathf.Abs(throttle);
        float speedRev = Mathf.Clamp01(speed / maxSpeed);

        float rev = Mathf.Max(throttleRev * 0.4f, speedRev);
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, rev);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, t);

        audioSource.pitch = Mathf.Lerp(
            audioSource.pitch,
            targetPitch,
            Time.deltaTime * pitchSmooth
        );

        audioSource.volume = Mathf.Lerp(
            audioSource.volume,
            targetVolume,
            Time.deltaTime * volumeSmooth
        );
    }
}