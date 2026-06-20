using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    //[SerializeField] private float verticalSpeed = 5f;
    [SerializeField] private float acceleration = 10f;

    [Header("Altitude")]
    [SerializeField] private float minAltitude = 2f;
    [SerializeField] private float maxAltitude = 20f;
    [SerializeField] private float altitudeSpeed = 3f;

    private float targetAltitude;

    [Header("Visual Tilt")]
    [SerializeField] private float maxTiltAngle = 25f;
    [SerializeField] private float tiltSmoothness = 5f;
    [SerializeField] private Transform droneBody;

    private Rigidbody rb;

    private Vector2 moveInput;
    private float heightInput;

    private float currentPitch;
    private float currentRoll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = 2f;
        rb.angularDamping = 5f;

        targetAltitude = transform.position.y;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnHeight(InputAction.CallbackContext context)
    {
        heightInput = context.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        HandleVisualTilt();
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
                1f)
            * altitudeSpeed;

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

    private void HandleVisualTilt()
    {
        float targetPitch = moveInput.y * maxTiltAngle;
        float targetRoll = -moveInput.x * maxTiltAngle;

        currentPitch = Mathf.Lerp(
            currentPitch,
            targetPitch,
            tiltSmoothness * Time.deltaTime);

        currentRoll = Mathf.Lerp(
            currentRoll,
            targetRoll,
            tiltSmoothness * Time.deltaTime);

        droneBody.localRotation =
            Quaternion.Euler(
                currentPitch,
                0f,
                currentRoll);
    }

    public void SetTargetAltitude(float sliderValue)
    {
        targetAltitude =
            Mathf.Lerp(
                minAltitude,
                maxAltitude,
                sliderValue);

        Debug.Log($"Target Altitude: {targetAltitude:F1}m");
    }
}