using UnityEngine;

public class CropField : MonoBehaviour
{
    [Header("Needs")]
    [SerializeField] private TreatmentRequirement water;
    [SerializeField] private TreatmentRequirement fertilizer;
    [SerializeField] private TreatmentRequirement pesticide;

    private Outline outline;

    private bool scanned;
    public bool IsCompleted =>
    !water.Needed &&
    !fertilizer.Needed &&
    !pesticide.Needed;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        outline.enabled = false;
    }

    private void Start()
    {
        water.Needed = Random.value > .5f;
        fertilizer.Needed = Random.value > .5f;
        pesticide.Needed = Random.value > .5f;

        water.OptimalAltitude = Random.Range(3f, 7f);
        fertilizer.OptimalAltitude = Random.Range(5f, 9f);
        pesticide.OptimalAltitude = Random.Range(2f, 5f);

        MissionManager.Instance.RegisterCrop(this);
        UpdateVisuals();
    }

    public void SetScanned(bool value)
    {
        scanned = value;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (!scanned)
        {
            outline.enabled = false;
            return;
        }

        outline.enabled = true;

        if (pesticide.Needed)
            outline.OutlineColor = Color.red;
        else if (fertilizer.Needed)
            outline.OutlineColor = Color.yellow;
        else if (water.Needed)
            outline.OutlineColor = Color.blue;
        else
            outline.OutlineColor = Color.green;
    }

    public bool IsAltitudeCorrect(SprayType spray, float altitude)
    {
        TreatmentRequirement treatment = GetRequirement(spray);

        if (!treatment.Needed)
            return false;

        return Mathf.Abs(
            altitude -
            treatment.OptimalAltitude)
            <= treatment.Tolerance;
    }

    public float GetOptimalAltitude(SprayType spray)
    {
        return GetRequirement(spray).OptimalAltitude;
    }

    public float GetTolerance(SprayType spray)
    {
        return GetRequirement(spray).Tolerance;
    }

    private TreatmentRequirement GetRequirement(SprayType spray)
    {
        switch (spray)
        {
            case SprayType.Water:
                return water;

            case SprayType.Fertilizer:
                return fertilizer;

            default:
                return pesticide;
        }
    }

    public void ReceiveTreatment(SprayType spray, float altitude)
    {
        Debug.Log($"[{name}] ReceiveTreatment called.");
        Debug.Log($"[{name}] Spray: {spray}");
        Debug.Log($"[{name}] Drone Altitude: {altitude:F2}");

        TreatmentRequirement treatment =
            GetRequirement(spray);

        Debug.Log($"[{name}] Needed: {treatment.Needed}");
        Debug.Log($"[{name}] Optimal Altitude: {treatment.OptimalAltitude:F2}");
        Debug.Log($"[{name}] Tolerance: ±{treatment.Tolerance:F2}");

        if (!treatment.Needed)
        {
            Debug.Log($"[{name}] Wrong spray type or already completed.");
            return;
        }

        if (!IsAltitudeCorrect(spray, altitude))
        {
            Debug.Log($"[{name}] Altitude incorrect!");
            return;
        }

        Debug.Log($"[{name}] Treatment SUCCESS!");

        treatment.Needed = false;

        UpdateVisuals();

        if (IsCompleted)
        {
            Debug.Log($"[{name}] Crop COMPLETED.");

            MissionManager.Instance.NotifyCropCompleted(this);
        }
    }
}