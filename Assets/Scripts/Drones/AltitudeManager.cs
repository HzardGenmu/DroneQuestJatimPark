using UnityEngine;

public class AltitudeManager : MonoBehaviour
{
    [SerializeField] private DroneController drone;
    [SerializeField] private DroneSprayer sprayer;
    [SerializeField] private LayerMask cropLayer;
    [SerializeField] private float checkDistance = 25f;
    [SerializeField] private float checkInterval = 0.05f;

    private float timer;

    public CropField CurrentCrop { get; private set; }

    public enum AltitudeState
    {
        None,
        TooLow,
        Correct,
        TooHigh
    }

    public AltitudeState CurrentState { get; private set; }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckCrop();
        }
    }

    void CheckCrop()
    {
        CurrentCrop = null;

        if (!Physics.Raycast(
            drone.transform.position,
            Vector3.down,
            out RaycastHit hit,
            checkDistance,
            cropLayer))
        {
            CurrentState = AltitudeState.None;
            return;
        }

        CurrentCrop =
            hit.collider.GetComponentInParent<CropField>();

        if (CurrentCrop == null)
        {
            CurrentState = AltitudeState.None;
            return;
        }

        float target =
            CurrentCrop.GetOptimalAltitude(
                sprayer.CurrentSprayType);

        float tolerance =
            CurrentCrop.GetTolerance(
                sprayer.CurrentSprayType);

        float current =
            drone.CurrentAltitude;

        if (current < target - tolerance)
        {
            CurrentState = AltitudeState.TooLow;
        }
        else if (current > target + tolerance)
        {
            CurrentState = AltitudeState.TooHigh;
        }
        else
        {
            CurrentState = AltitudeState.Correct;
        }
    }
}