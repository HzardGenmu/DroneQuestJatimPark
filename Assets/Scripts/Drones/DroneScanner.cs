using UnityEngine;

public class DroneScanner : MonoBehaviour
{
    [SerializeField] private Transform scanOrigin;

    [SerializeField] private float scanRadius = 2f;
    [SerializeField] private float scanHeight = 10f;

    [SerializeField] private LayerMask cropLayer;

    public CropField CurrentScannedCrop { get; private set; }

    private CropField previousCrop;

    private void Update()
    {
        Scan();
    }

    private void Scan()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            scanOrigin.position,
            scanRadius,
            -scanOrigin.up,
            scanHeight,
            cropLayer);

        CropField bestCrop = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            CropField crop =
                hit.collider.GetComponentInParent<CropField>();

            if (crop == null)
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                bestCrop = crop;
            }
        }

        if (previousCrop != bestCrop)
        {
            if (previousCrop != null)
                previousCrop.SetScanned(false);

            if (bestCrop != null)
                bestCrop.SetScanned(true);

            previousCrop = bestCrop;
        }

        CurrentScannedCrop = bestCrop;
    }

    private void OnDrawGizmos()
    {
        if (scanOrigin == null)
            return;

        Gizmos.color = Color.yellow;

        Vector3 start = scanOrigin.position;
        Vector3 end = start + (-scanOrigin.up * scanHeight);

        Gizmos.DrawLine(start, end);

        DrawWireCircle(start, scanRadius);
        DrawWireCircle(end, scanRadius);

        Gizmos.DrawLine(start + scanOrigin.right * scanRadius,
                        end + scanOrigin.right * scanRadius);

        Gizmos.DrawLine(start - scanOrigin.right * scanRadius,
                        end - scanOrigin.right * scanRadius);

        Gizmos.DrawLine(start + scanOrigin.forward * scanRadius,
                        end + scanOrigin.forward * scanRadius);

        Gizmos.DrawLine(start - scanOrigin.forward * scanRadius,
                        end - scanOrigin.forward * scanRadius);
    }

    private void DrawWireCircle(Vector3 center, float radius)
    {
        const int segments = 32;

        Vector3 prev = center + scanOrigin.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 next =
                center +
                (scanOrigin.right * Mathf.Cos(angle) +
                 scanOrigin.forward * Mathf.Sin(angle)) * radius;

            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}