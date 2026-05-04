using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class WheelSetup
{
    public Transform pos;
    public bool isFront;
    public bool isRight;

    public UnityEvent<float> setGripUsed;

    [Header("Calculations")] 
    public float rightSpeed;

    public float rightForce;

    public float gripUsed;
}

public class CarController : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rb;
    
    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference brakeAction; // optional

    [Header("Tuning")]
    public float engineForce = 500f;
    public float engineDragMultiplier = 1.0f;
    
    public float forwardDragMultiplier = 2f;
    
    /// <summary>
    /// Side drag should be higher than forward drag (cars are more streamlined)
    /// </summary>
    public float sideDragMultiplier = 4f;

    public float grip = 8f;
    public float maxGripForce = 1000f;
    
    public float steerAngle = 45f;
    public float steerResponse = 10f;
    
    public WheelSetup[] wheels;


    public UnityEvent<float> setSteering;
    public UnityEvent<float> setThrottle;
    public UnityEvent<float> setGripUsed;

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

    float AllocateGrip(float grip, ref float thrust, ref float side)
    {
        if (thrust > grip)
        {
            thrust = grip;
            side = 0;
            return 0;
        }
        else
        {
            var gripLeft = grip - thrust;
            if (side > gripLeft)
            {
                side = gripLeft;
                return 0;
            }
            else
            {
                return gripLeft - side;
            }
        }
    }

    void ApplyWheel(WheelSetup wheel, Vector2 input)
    {
        Vector3 forward = rb.transform.forward;
        var thrust = input.y * engineForce;

        if (wheel.isFront)
        {
            forward = Quaternion.AngleAxis(steering * steerAngle, Vector3.up) * forward;
        }
        else
        {
            thrust = 0;
        }
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        
        Vector3 velocity = rb.GetPointVelocity(wheel.pos.position);
        float forwardSpeed = Vector3.Dot(velocity, forward);

        var engineDrag = -engineDragMultiplier * forwardSpeed;
        
        wheel.rightSpeed = Vector3.Dot(velocity, right);
        
        var sideForce = Mathf.Abs(wheel.rightSpeed) * grip;
        
        // Assumption: thrust and sideForce are positive
        float gripLeft = AllocateGrip(maxGripForce, ref thrust, ref sideForce);

        var forwardForce = thrust + engineDrag;
        wheel.rightForce = -Mathf.Sign(wheel.rightSpeed) * sideForce;

        wheel.gripUsed = (maxGripForce - gripLeft) / maxGripForce;
        wheel.setGripUsed.Invoke(wheel.gripUsed);
        
        rb.AddForceAtPosition(forwardForce * forward + wheel.rightForce * right, wheel.pos.position, ForceMode.Force);
    }
    
    void FixedUpdate()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        
        // Don't move if the car is on the side or upside down
        float uprightAmount = Vector3.Dot(transform.up, Vector3.up);
        bool onSideOrUpsideDown = uprightAmount < 0.5f;
        if (onSideOrUpsideDown) input = Vector2.zero;

        // moveInput = input;

        var throttle = input.y;
        setThrottle.Invoke(throttle);
        // var steering = input.x;

        var targetSteer = input.x;
        steering = Mathf.MoveTowards(steering, targetSteer, Time.fixedDeltaTime * steerResponse);
        setSteering.Invoke(steering);
        
        foreach(var w in wheels) ApplyWheel(w, input);
        
        var gripUsed = Mathf.Max(wheels[0].gripUsed, wheels[1].gripUsed, wheels[2].gripUsed, wheels[3].gripUsed);
        setGripUsed.Invoke(gripUsed);

        Vector3 velocity = rb.linearVelocity;

        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        float sideSpeed    = Vector3.Dot(velocity, transform.right);

        Vector3 forwardDrag = -transform.forward * (forwardSpeed * Mathf.Abs(forwardSpeed) * forwardDragMultiplier);
        Vector3 sideDrag    = -transform.right * (sideSpeed * Mathf.Abs(sideSpeed) * sideDragMultiplier);

        rb.AddForce(forwardDrag + sideDrag, ForceMode.Force);

    }
}