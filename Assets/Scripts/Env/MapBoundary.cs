using UnityEngine;

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

    private void Update()
    {
        float distance =
            Vector3.Distance(
                drone.position,
                mapCenter);

        if (distance < warningRadius)
        {
            staticEffect.SetIntensity(0f);
            return;
        }

        float t =
            Mathf.InverseLerp(
                warningRadius,
                respawnRadius,
                distance);

        staticEffect.SetIntensity(t);

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
}