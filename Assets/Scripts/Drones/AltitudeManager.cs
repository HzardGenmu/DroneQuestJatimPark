using UnityEngine;

public class AltitudeManager : MonoBehaviour
{
    [SerializeField] private DroneController drone;
    //[SerializeField] private DroneSprayer sprayer;
    //[SerializeField] private LayerMask cropLayer;
    //[SerializeField] private float checkDistance = 25f;
    [SerializeField] private float checkInterval = 0.05f;

    private DroneScanner scanner;
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
    public AltitudeState SelectedState { get; private set; }

    private void Awake()
    {
        scanner = FindAnyObjectByType<DroneScanner>();

        if (scanner == null)
        {
            Debug.LogError("No DroneScanner found in the scene!", this);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckCrop();
        }
    }

    //private void CheckCrop()
    //{
    //    CurrentCrop = scanner.CurrentScannedCrop;

    //    if (CurrentCrop == null)
    //    {
    //        CurrentState = AltitudeState.None;
    //        SelectedState = AltitudeState.None;
    //        return;
    //    }

    //    if ((CropTreatment)sprayer.CurrentSprayType !=
    //        CurrentCrop.GetRequiredTreatment())
    //    {
    //        CurrentState = AltitudeState.None;
    //        SelectedState = AltitudeState.None;
    //        return;
    //    }

    //    float target = CurrentCrop.GetOptimalAltitude();
    //    float tolerance = CurrentCrop.GetTolerance();

    //    CurrentState =
    //        GetState(drone.CurrentAltitude, target, tolerance);

    //    SelectedState =
    //        GetState(drone.TargetAltitude, target, tolerance);
    //}

    private void CheckCrop()
    {
        CurrentCrop = scanner.CurrentScannedCrop;

        if (CurrentCrop == null)
        {
            CurrentState = AltitudeState.None;
            SelectedState = AltitudeState.None;
            return;
        }

        float target = CurrentCrop.GetOptimalAltitude();
        float tolerance = CurrentCrop.GetTolerance();

        CurrentState =
            GetState(
                drone.CurrentAltitude,
                target,
                tolerance);

        SelectedState =
            GetState(
                drone.TargetAltitude,
                target,
                tolerance);
    }

    private AltitudeState GetState(
    float altitude,
    float target,
    float tolerance)
    {
        if (altitude < target - tolerance)
            return AltitudeState.TooLow;

        if (altitude > target + tolerance)
            return AltitudeState.TooHigh;

        return AltitudeState.Correct;
    }
}