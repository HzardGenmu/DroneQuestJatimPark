using UnityEngine;

public class DroneScanner : MonoBehaviour
{
    [SerializeField] private Transform scanOrigin;

    [SerializeField] private float scanDistance = 10f;
    [SerializeField] private float scanAngle = 30f;

    [SerializeField] private LayerMask cropLayer;

    private void Update()
    {
        Scan();
    }

    private void Scan()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                scanOrigin.position,
                scanDistance,
                cropLayer);

        foreach (Collider hit in hits)
        {
            Vector3 direction =
                (hit.transform.position - scanOrigin.position).normalized;

            float angle =
                Vector3.Angle(
                    -scanOrigin.up,
                    direction);

            CropField crop =
                hit.GetComponent<CropField>();

            if (crop == null)
                continue;

            if (angle <= scanAngle)
            {
                crop.SetScanned(true);
            }
            else
            {
                crop.SetScanned(false);
            }
        }
    }

    private void OnDrawGizmos()
{
    if (scanOrigin == null)
        return;

    Gizmos.color = Color.yellow;

    Gizmos.DrawLine(
        scanOrigin.position,
        scanOrigin.position +
        (-scanOrigin.up * scanDistance));
}
}