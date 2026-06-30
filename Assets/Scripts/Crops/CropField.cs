using UnityEngine;

public class CropField : MonoBehaviour
{
    [Header("Needs")]
    [SerializeField] private TreatmentRequirement water;
    [SerializeField] private TreatmentRequirement fertilizer;
    [SerializeField] private TreatmentRequirement pesticide;

    private Outline outline;

    private bool scanned;

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
        TreatmentRequirement treatment =
            GetRequirement(spray);

        if (!treatment.Needed)
            return;

        if (!IsAltitudeCorrect(spray, altitude))
            return;

        treatment.Needed = false;

        UpdateVisuals();
    }
}