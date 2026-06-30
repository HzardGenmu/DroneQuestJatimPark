using UnityEngine;
using UnityEngine.UI;

public class MapBoundary : MonoBehaviour
{
    [Header("Boundary")]
    [SerializeField] private Transform drone;

    [SerializeField] private Vector3 mapCenter;

    [SerializeField] private float warningRadius = 90f;
    [SerializeField] private float respawnRadius = 120f;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    [Header("Effects")]
    [SerializeField] private SignalStaticEffect staticEffect;

    [Header("UI")]
    [SerializeField] private Image signalIcon;

    [SerializeField] private Sprite signal4;
    [SerializeField] private Sprite signal3;
    [SerializeField] private Sprite signal2;
    [SerializeField] private Sprite signal1;
    private void Update()
    {
        float distance =
            Vector3.Distance(
                drone.position,
                mapCenter);

        if (distance < warningRadius)
        {
            staticEffect.SetIntensity(0f);

            signalIcon.sprite = signal4;

            return;
        }

        float t =
            Mathf.InverseLerp(
                warningRadius,
                respawnRadius,
                distance);

        staticEffect.SetIntensity(t);

        UpdateSignalIcon(t);

        if (distance >= respawnRadius)
        {
            RespawnDrone();
        }
    }

    private void RespawnDrone()
    {
        drone.position =
            respawnPoint.position;

        Rigidbody rb =
            drone.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        staticEffect.SetIntensity(0f);

        Debug.Log("Drone lost signal. Returning to base.");
    }

    private void UpdateSignalIcon(float signalStrength)
    {
        if (signalStrength < 0.25f)
            signalIcon.sprite = signal4;
        else if (signalStrength < 0.5f)
            signalIcon.sprite = signal3;
        else if (signalStrength < 0.75f)
            signalIcon.sprite = signal2;
        else
            signalIcon.sprite = signal1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(mapCenter, warningRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mapCenter, respawnRadius);

        if (respawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(respawnPoint.position, 1f);
        }
    }
}