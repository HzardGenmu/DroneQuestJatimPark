using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class CropTreatmentIcon : MonoBehaviour
{
    [Header("Icon")]
    [SerializeField] private Image iconRenderer;

    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite fertilizerIcon;
    [SerializeField] private Sprite pesticideIcon;

    [Header("Billboard")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private DroneCamera droneCamera;

    [Header("Bottom Camera")]
    [SerializeField] private float bottomCameraTilt = 90f;

    private float originalYRotation;

    private void Awake()
    {
        // Remember the icon's original horizontal orientation.
        originalYRotation = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (brain == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
                brain = mainCamera.GetComponent<CinemachineBrain>();
        }

        if (droneCamera == null)
        {
            droneCamera = FindAnyObjectByType<DroneCamera>();
        }

        if (brain == null || droneCamera == null)
            return;

        Camera activeCamera = brain.OutputCamera;

        if (activeCamera == null)
            return;

        if (droneCamera.IsBottomCameraActive)
        {
            UpdateBottomCameraRotation();
        }
        else
        {
            UpdateFollowCameraRotation(activeCamera);
        }
    }

    private void UpdateFollowCameraRotation(Camera activeCamera)
    {
        Vector3 direction =
            activeCamera.transform.position - transform.position;

        // Ignore vertical difference.
        // The icon only rotates horizontally.
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.rotation =
            Quaternion.LookRotation(direction);
    }

    private void UpdateBottomCameraRotation()
    {
        // Reset horizontal rotation to the original orientation.
        Quaternion horizontalRotation =
            Quaternion.Euler(
                0f,
                originalYRotation,
                0f
            );

        // Pivot upward/downward from that original orientation.
        Quaternion tilt =
            Quaternion.Euler(
                bottomCameraTilt,
                0f,
                0f
            );

        transform.rotation =
            horizontalRotation * tilt;
    }

    public void SetTreatment(CropTreatment treatment)
    {
        switch (treatment)
        {
            case CropTreatment.Water:
                iconRenderer.sprite = waterIcon;
                break;

            case CropTreatment.Fertilizer:
                iconRenderer.sprite = fertilizerIcon;
                break;

            case CropTreatment.Pesticide:
                iconRenderer.sprite = pesticideIcon;
                break;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}