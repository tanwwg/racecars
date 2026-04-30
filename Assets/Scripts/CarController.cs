using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rb;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference brakeAction; // optional

    [Header("Tuning")]
    public float engineForce = 500f;
    public float turnTorque = 500f;
    public float engineDragMultiplier = 1.0f;
    public float staticEngineDrag;
    public float grip = 8f;
    public float drag = 2f;

    public float lateralGrip = 5f;
    
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

    public float thrust;
    public float forwardTurnDrag;
    public float forwardDrag;
    public float engineDrag;

    public float forwardVel;
    public float lateralVel;

    public float forwardForce;
    public float lateralForce;
    public Vector3 torqueForce;
    
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
        
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 velocity = rb.linearVelocity;
        forwardVel = Vector3.Dot(velocity, transform.forward);
        lateralVel = Vector3.Dot(velocity, right);

        // Engine
        this.thrust = throttle * engineForce;
        this.forwardTurnDrag = Mathf.Abs(lateralVel) * grip;
        this.forwardDrag = Mathf.Pow(forwardVel, 2) * drag;
        engineDrag = forwardVel * engineDragMultiplier;

        this.forwardForce = thrust - forwardTurnDrag - forwardDrag - engineDrag - staticEngineDrag;
        this.lateralForce = lateralVel * lateralGrip;

        rb.AddForce(forwardForce * forward - lateralForce * right, ForceMode.Force);

        this.torqueForce = transform.up * (steering * turnTorque * velocity.magnitude);
        rb.AddTorque(torqueForce, ForceMode.Force);
    }
}