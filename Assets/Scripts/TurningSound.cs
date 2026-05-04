using UnityEngine;

public class TurningSound : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource screechSource;

    public float slipStart = 2f;   // start screech
    public float slipMax = 10f;    // full volume

    public float slip;

    public AnimationCurve volumeCurve;
    public AnimationCurve pitchCurve;

    public void SetGripUsed(float grip)
    {
        screechSource.volume = volumeCurve.Evaluate(grip);
        screechSource.pitch = pitchCurve.Evaluate(grip);
        if (screechSource.volume > 0.05f)
        {
            if (!screechSource.isPlaying)
                screechSource.Play();
        }
        else
        {
            screechSource.Stop();
        }
    }
    
}
