using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class WheelSetup
{
    public Transform pos;
    public bool isFront;
    public bool isRight;

    [Header("Calculations")] 
    public float rightSpeed;

    public float rightForce;
}

public class CarController : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rb;

    public WheelSetup[] wheels;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference brakeAction; // optional

    [Header("Tuning")]
    public float engineForce = 500f;
    public float turnTorque = 500f;
    public float engineDragMultiplier = 1.0f;
    public float staticEngineDrag;
    public float drag = 2f;

    public float grip = 8f;
    public float maxGripForce = 1000f;
    
    public float steerAngle = 45f;
    public float steerResponse = 10f;

    public UnityEvent<float> setSteering;
    public UnityEvent<float> setThrottle;    

    void OnEnable()
    {
        moveAction.action.Enable();
        if (brakeAction != null)
            brakeAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        if (brakeAction != null)
            brakeAction.action.Disable();
    }

    [Header("Calculations")]
    public float steering;

    void ApplyWheel(WheelSetup wheel, Vector2 input)
    {
        Vector3 forward = rb.transform.forward;
        if (wheel.isFront) forward = Quaternion.AngleAxis(steering * steerAngle, Vector3.up) * forward;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        
        Vector3 velocity = rb.GetPointVelocity(wheel.pos.position);
        float forwardSpeed = Vector3.Dot(velocity, forward);

        var thrust = input.y * engineForce;
        var airDrag = -Mathf.Pow(forwardSpeed, 2) * drag;
        var engineDrag = -engineDragMultiplier * forwardSpeed;
        var forwardForce = thrust + airDrag + engineDrag;
        
        wheel.rightSpeed = Vector3.Dot(velocity, right);
        wheel.rightForce = Mathf.Clamp(-wheel.rightSpeed * grip, -maxGripForce, maxGripForce);
        
        rb.AddForceAtPosition(forwardForce * forward + wheel.rightForce * right, wheel.pos.position, ForceMode.Force);
    }
    
    void FixedUpdate()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // moveInput = input;

        var throttle = input.y;
        setThrottle.Invoke(throttle);
        // var steering = input.x;

        var targetSteer = input.x;
        steering = Mathf.MoveTowards(steering, targetSteer, Time.fixedDeltaTime * steerResponse);
        setSteering.Invoke(steering);
        
        foreach(var w in wheels) ApplyWheel(w, input);
    }
}