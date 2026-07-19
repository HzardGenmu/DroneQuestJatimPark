using UnityEngine;

public class AltitudeManager : MonoBehaviour
{
    [SerializeField] private DroneController drone;
    [SerializeField] private DroneSprayer sprayer;
    [SerializeField] private LayerMask cropLayer;
    [SerializeField] private float checkDistance = 25f;
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

    private void CheckCrop()
    {
        CurrentCrop = scanner.CurrentScannedCrop;

        if (CurrentCrop == null)
        {
            CurrentState = AltitudeState.None;
            return;
        }

        if ((CropTreatment)sprayer.CurrentSprayType !=
            CurrentCrop.GetRequiredTreatment())
        {
            CurrentState = AltitudeState.None;
            return;
        }

        float target = CurrentCrop.GetOptimalAltitude();
        float tolerance = CurrentCrop.GetTolerance();

        float current = drone.CurrentAltitude;

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