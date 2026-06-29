using DG.Tweening;
using UnityEngine;

public class DroneIdle : MonoBehaviour
{
    [Header("Hover")]
    [SerializeField] private float hoverHeight = 0.08f;
    [SerializeField] private float hoverSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Tilt")]
    [SerializeField] private float tiltAmount = 2f;
    [SerializeField] private float tiltSpeed = 1.5f;

    private Vector3 startPosition;

    public bool EnableIdle { get; set; } = true;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (!EnableIdle)
            return;

        // Hover
        transform.localPosition =
            startPosition +
            Vector3.up *
            Mathf.Sin(Time.time * hoverSpeed) *
            hoverHeight;

        // Slow rotation
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World);

        // Small hover tilt
        float pitch =
            Mathf.Sin(Time.time * tiltSpeed) *
            tiltAmount;

        float roll =
            Mathf.Cos(Time.time * tiltSpeed * 0.8f) *
            tiltAmount;

        transform.localRotation =
            Quaternion.Euler(
                pitch,
                transform.localEulerAngles.y,
                roll);
    }
}