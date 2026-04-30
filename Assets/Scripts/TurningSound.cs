using UnityEngine;

public class TurningSound : MonoBehaviour
{
    public Rigidbody rb;
    public AudioSource screechSource;

    public float slipStart = 2f;   // start screech
    public float slipMax = 10f;    // full volume

    public float slip;
    
    
    void Update()
    {
        this.slip = Mathf.Abs(Vector3.Dot(rb.linearVelocity, rb.transform.right));

        float t = Mathf.InverseLerp(slipStart, slipMax, slip);

        screechSource.volume = t;
        screechSource.pitch = Mathf.Lerp(0.8f, 1.5f, t);

        if (t > 0.05f)
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
