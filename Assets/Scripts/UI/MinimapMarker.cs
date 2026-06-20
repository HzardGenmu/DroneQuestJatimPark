using UnityEngine;

public class MinimapMarker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform drone;
    [SerializeField] private RectTransform droneIcon;

    [Header("World Bounds")]
    [SerializeField] private Vector2 worldMin;
    [SerializeField] private Vector2 worldMax;

    [Header("Map Size")]
    [SerializeField] private RectTransform mapRect;

    private void Update()
    {
        float normalizedX =
            Mathf.InverseLerp(
                worldMin.x,
                worldMax.x,
                drone.position.x);

        float normalizedY =
            Mathf.InverseLerp(
                worldMin.y,
                worldMax.y,
                drone.position.z);

        droneIcon.rotation =
    Quaternion.Euler(
        0,
        0,
        -drone.eulerAngles.y);

        float mapX =
            (normalizedX * mapRect.rect.width)
            - (mapRect.rect.width * 0.5f);

        float mapY =
            (normalizedY * mapRect.rect.height)
            - (mapRect.rect.height * 0.5f);

        droneIcon.anchoredPosition =
            new Vector2(mapX, mapY);
    }
}