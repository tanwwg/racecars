using UnityEngine;

[ExecuteInEditMode]
public class TurnMeter : MonoBehaviour
{
    public Transform targetTransform;
    
    /// <summary>
    /// Set to -1 to 1
    /// </summary>
    public float turn;

    public float maxAngle = 45;
    
    public void SetTurn(float t) => this.turn = t;
    
    void Update()
    {
        targetTransform.localRotation = Quaternion.Euler(0, 0, turn * maxAngle);
    }
}
