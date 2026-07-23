using UnityEngine;

public class MainMenuEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform spotlightTransform;

    [Header("Camera")]
    [SerializeField] private float cameraMaxAngle = 8f;
    [SerializeField] private float cameraSmooth = 5f;

    [Header("Spotlight")]
    [SerializeField] private float swingAngle = 15f;
    [SerializeField] private float swingSpeed = 0.6f;
    [SerializeField] private float phoneInfluence = 20f;
    [SerializeField] private float spotlightSmooth = 5f;

    private Quaternion cameraStartRotation;
    private Quaternion spotlightStartRotation;

    private void Start()
    {
#if !UNITY_EDITOR
        Input.gyro.enabled = true;
#endif

        cameraStartRotation = cameraTransform.localRotation;
        spotlightStartRotation = spotlightTransform.localRotation;
    }

    private void Update()
    {
        UpdateCamera();
        UpdateSpotlight();
    }

    private void UpdateCamera()
    {
#if UNITY_EDITOR
        return;
#endif

        Vector3 gravity = Input.gyro.gravity;

        Quaternion target =
            cameraStartRotation *
            Quaternion.Euler(
                -gravity.y * cameraMaxAngle,
                gravity.x * cameraMaxAngle,
                0f);

        cameraTransform.localRotation =
            Quaternion.Slerp(
                cameraTransform.localRotation,
                target,
                cameraSmooth * Time.deltaTime);
    }

    private void UpdateSpotlight()
    {
        float idle =
            Mathf.Sin(Time.time * swingSpeed) *
            swingAngle;

        Vector3 accel = Input.acceleration;

        float movement =
            Mathf.Abs(accel.x) +
            Mathf.Abs(accel.y);

        float phoneOffset =
            accel.x *
            phoneInfluence *
            (1f + movement);

        Quaternion target =
            spotlightStartRotation *
            Quaternion.Euler(
                0f,
                idle + phoneOffset,
                0f);

        spotlightTransform.localRotation =
            Quaternion.Slerp(
                spotlightTransform.localRotation,
                target,
                spotlightSmooth * Time.deltaTime);
    }
}