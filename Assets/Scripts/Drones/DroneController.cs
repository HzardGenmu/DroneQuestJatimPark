using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    public enum DroneState
    {
        TakingOff,
        Flying,
        Landing,
        Landed
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 10f;

    [Header("Altitude")]
    [SerializeField] private float minAltitude = 2f;
    [SerializeField] private float maxAltitude = 20f;
    [SerializeField] private float altitudeSpeed = 3f;

    [Header("Landing")]
    [SerializeField] private float landingHeight = 0.25f;
    [SerializeField] private float landingMoveSpeed = 3f;
    [SerializeField] private float landingDetectionDistance = 8f;
    [SerializeField] private LayerMask helipadLayer;

    [Header("Visual Tilt")]
    [SerializeField] private float maxTiltAngle = 25f;
    [SerializeField] private float tiltSmoothness = 5f;
    [SerializeField] private Transform droneBody;

    [Header("Joystick")]
    [SerializeField] private FloatingJoystick joystick;

    public bool IsLandingAvailable => CanLand();
    public bool IsBusy =>
    CurrentState == DroneState.TakingOff ||
    CurrentState == DroneState.Landing;
    public float CurrentAltitude => transform.position.y;
    public float MinAltitude => minAltitude;
    public float MaxAltitude => maxAltitude;
    public float TargetAltitude => targetAltitude;

    public DroneState CurrentState { get; private set; }

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;
    public Vector2 LookInput => lookInput;

    private float targetAltitude;

    private float currentPitch;
    private float currentRoll;

    private Helipad currentHelipad;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = 2f;
        rb.angularDamping = 5f;

        targetAltitude = 3f;

        CurrentState = DroneState.TakingOff;
    }

    private void Start()
    {
        GameEvents.OnDroneTakeoff?.Invoke();
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case DroneState.TakingOff:
                HandleTakeoff();
                break;

            case DroneState.Flying:
                HandleMovement();
                break;

            case DroneState.Landing:
                HandleLanding();
                break;

            case DroneState.Landed:
                rb.linearVelocity = Vector3.zero;
                break;
        }
    }

    private void Update()
    {
        if (CurrentState == DroneState.Flying)
        {
            HandleVisualTilt();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (CurrentState != DroneState.Flying)
            return;

        moveInput = context.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        Vector2 input = moveInput;

        if (joystick != null && joystick.IsDragging)
        {
            input = joystick.Input;
        }

        Vector3 horizontalVelocity =
            transform.forward * input.y +
            transform.right * input.x;

        horizontalVelocity *= moveSpeed;

        float altitudeDifference =
            targetAltitude - transform.position.y;

        float verticalVelocity =
            altitudeDifference * altitudeSpeed;

        verticalVelocity =
            Mathf.Clamp(verticalVelocity, -12f, 12f);

        Vector3 velocity =
            rb.linearVelocity;

        velocity.x = Mathf.Lerp(
            velocity.x,
            horizontalVelocity.x,
            acceleration * Time.fixedDeltaTime);

        velocity.z = Mathf.Lerp(
            velocity.z,
            horizontalVelocity.z,
            acceleration * Time.fixedDeltaTime);

        // No smoothing on Y
        velocity.y = verticalVelocity;

        rb.linearVelocity = velocity;
    }

    private void HandleTakeoff()
    {
        float difference =
            targetAltitude - transform.position.y;

        rb.linearVelocity =
            new Vector3(
                0,
                difference * altitudeSpeed,
                0);

        if (Mathf.Abs(difference) < 0.2f)
        {
            CurrentState = DroneState.Flying;

            CanLand();
        }
    }

    private void HandleLanding()
    {
        if (currentHelipad == null)
            return;

        Vector3 padCenter =
            currentHelipad.transform.position;

        Vector3 horizontalOffset =
            padCenter - transform.position;

        horizontalOffset.y = 0f;

        Vector3 horizontalVelocity =
            horizontalOffset.normalized *
            landingMoveSpeed;

        if (horizontalOffset.magnitude < 0.2f)
        {
            horizontalVelocity = Vector3.zero;
        }

        Vector3 velocity =
            horizontalVelocity;

        velocity.y = -2f;

        rb.linearVelocity = velocity;

        float distanceToCenter =
            horizontalOffset.magnitude;

        float padHeight =
            currentHelipad.transform.position.y;

        if (distanceToCenter < 0.2f &&
            transform.position.y <= padHeight + landingHeight)
        {
            rb.linearVelocity = Vector3.zero;

            CurrentState = DroneState.Landed;
        }
    }

    public void SetTargetAltitude(float altitude)
    {
        if (CurrentState != DroneState.Flying)
            return;

        targetAltitude =
            Mathf.Clamp(
                altitude,
                minAltitude,
                maxAltitude);
    }

    public bool CanLand()
    {
        if (CurrentState != DroneState.Flying)
            return false;

        Debug.DrawRay(
            transform.position,
            Vector3.down * landingDetectionDistance,
            Color.red);

        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit hit,
            landingDetectionDistance,
            helipadLayer))
        {
            currentHelipad =
                hit.collider.GetComponent<Helipad>();
            return currentHelipad != null;
        }

        currentHelipad = null;
        return false;
    }

    public void BeginLanding()
    {
        if (!CanLand())
            return;

        CurrentState = DroneState.Landing;

        GameEvents.OnDroneLanding?.Invoke();
    }

    public void BeginTakeoff()
    {
        if (CurrentState != DroneState.Landed)
            return;

        targetAltitude = 3f;

        CurrentState = DroneState.TakingOff;

        GameEvents.OnDroneTakeoff?.Invoke();
    }

    private void HandleVisualTilt()
    {
        Vector2 input = moveInput;

        if (joystick != null && joystick.IsDragging)
        {
            input = joystick.Input;
        }

        float targetPitch =
            input.y * maxTiltAngle;

        float targetRoll =
            -input.x * maxTiltAngle;

        currentPitch =
            Mathf.Lerp(
                currentPitch,
                targetPitch,
                tiltSmoothness * Time.deltaTime);

        currentRoll =
            Mathf.Lerp(
                currentRoll,
                targetRoll,
                tiltSmoothness * Time.deltaTime);

        droneBody.localRotation =
            Quaternion.Euler(
                currentPitch,
                0f,
                currentRoll);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}