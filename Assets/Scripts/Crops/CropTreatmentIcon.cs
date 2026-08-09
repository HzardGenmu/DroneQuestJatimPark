using UnityEngine;
using UnityEngine.UI;

public class CropTreatmentIcon : MonoBehaviour
{
    [Header("Icon")]
    [SerializeField] private Image iconRenderer;

    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite fertilizerIcon;
    [SerializeField] private Sprite pesticideIcon;

    [Header("Billboard")]
    [SerializeField] private Transform player;

    [SerializeField] private bool rotateOnlyHorizontally = true;

    private void LateUpdate()
    {
        if (player == null)
        {
            DroneController drone =
                FindAnyObjectByType<DroneController>();

            if (drone != null)
                player = drone.transform;
        }

        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        if (rotateOnlyHorizontally)
            direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.rotation =
            Quaternion.LookRotation(direction);
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