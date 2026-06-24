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

    public bool IsLandingAvailable => CanLand();
    public bool IsBusy =>
    CurrentState == DroneState.TakingOff ||
    CurrentState == DroneState.Landing;

    public DroneState CurrentState { get; private set; }

    private Rigidbody rb;

    private Vector2 moveInput;

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
        Vector3 horizontalVelocity =
            transform.forward * moveInput.y +
            transform.right * moveInput.x;

        horizontalVelocity *= moveSpeed;

        float altitudeDifference =
            targetAltitude - transform.position.y;

        float verticalVelocity =
            Mathf.Clamp(
                altitudeDifference,
                -1f,
                1f) * altitudeSpeed;

        Vector3 targetVelocity =
            new Vector3(
                horizontalVelocity.x,
                verticalVelocity,
                horizontalVelocity.z);

        rb.linearVelocity = Vector3.Lerp(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime);
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

    public void SetTargetAltitude(float sliderValue)
    {
        if (CurrentState != DroneState.Flying)
            return;

        targetAltitude =
            Mathf.Lerp(
                minAltitude,
                maxAltitude,
                sliderValue);
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

        CurrentState =
            DroneState.Landing;
    }

    public void BeginTakeoff()
    {
        if (CurrentState != DroneState.Landed)
            return;

        targetAltitude = 3f;

        CurrentState =
            DroneState.TakingOff;
    }

    private void HandleVisualTilt()
    {
        float targetPitch =
            moveInput.y * maxTiltAngle;

        float targetRoll =
            -moveInput.x * maxTiltAngle;

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
}