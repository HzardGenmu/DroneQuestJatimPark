using UnityEngine;

public enum CropTreatment
{
    Water,
    Fertilizer,
    Pesticide
}

public class CropField : MonoBehaviour
{
    [Header("Treatment")]
    [SerializeField] private TreatmentRequirement requirement;
    [SerializeField] private CropTreatment requiredTreatment;

    [Header("Models")]
    [SerializeField] private GameObject healthyModel;
    [SerializeField] private GameObject fertilizerModel;
    [SerializeField] private GameObject pesticideModel;

    private Outline outline;
    private bool scanned;

    private static bool altitudesInitialized;

    private static float waterAltitude;
    private static float fertilizerAltitude;
    private static float pesticideAltitude;
    public bool IsCompleted => !requirement.Needed;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;

        requiredTreatment = (CropTreatment)Random.Range(0, 3);

        requirement.Needed = true;

        // Original logic.
        // requirement.OptimalAltitude = Random.Range(2f, 20f);

        // Generate one random altitude per treatment type.
        if (!altitudesInitialized)
        {
            waterAltitude = Random.Range(2f, 20f);
            fertilizerAltitude = Random.Range(2f, 20f);
            pesticideAltitude = Random.Range(2f, 20f);

            altitudesInitialized = true;

            Debug.Log($"Water Altitude: {waterAltitude:F1}");
            Debug.Log($"Fertilizer Altitude: {fertilizerAltitude:F1}");
            Debug.Log($"Pesticide Altitude: {pesticideAltitude:F1}");
        }

        switch (requiredTreatment)
        {
            case CropTreatment.Water:
                requirement.OptimalAltitude = waterAltitude;
                break;

            case CropTreatment.Fertilizer:
                requirement.OptimalAltitude = fertilizerAltitude;
                break;

            case CropTreatment.Pesticide:
                requirement.OptimalAltitude = pesticideAltitude;
                break;
        }

        // Ensure all models stay active so Outline caches every renderer.
        if (healthyModel != null)
            healthyModel.SetActive(true);

        if (fertilizerModel != null)
            fertilizerModel.SetActive(true);

        if (pesticideModel != null)
            pesticideModel.SetActive(true);
    }

    private void Start()
    {
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
        if (outline != null)
            outline.enabled = scanned;

        if (!requirement.Needed)
        {
            if (outline != null)
                outline.OutlineColor = Color.green;

            ShowHealthy();

            return;
        }

        switch (requiredTreatment)
        {
            case CropTreatment.Water:

                if (outline != null)
                    outline.OutlineColor = Color.blue;

                ShowHealthy();
                break;

            case CropTreatment.Fertilizer:

                if (outline != null)
                    outline.OutlineColor = Color.yellow;

                ShowFertilizer();
                break;

            case CropTreatment.Pesticide:

                if (outline != null)
                    outline.OutlineColor = Color.red;

                ShowPesticide();
                break;
        }
    }

    #region Model Switching

    private void ShowHealthy()
    {
        SetModelVisible(healthyModel, true);
        SetModelVisible(fertilizerModel, false);
        SetModelVisible(pesticideModel, false);
    }

    private void ShowFertilizer()
    {
        SetModelVisible(healthyModel, false);
        SetModelVisible(fertilizerModel, true);
        SetModelVisible(pesticideModel, false);
    }

    private void ShowPesticide()
    {
        SetModelVisible(healthyModel, false);
        SetModelVisible(fertilizerModel, false);
        SetModelVisible(pesticideModel, true);
    }

    private void SetModelVisible(GameObject model, bool visible)
    {
        if (model == null)
            return;

        Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
            r.enabled = visible;
    }

    #endregion

    public bool IsAltitudeCorrect(float altitude)
    {
        return Mathf.Abs(
            altitude - requirement.OptimalAltitude)
            <= requirement.Tolerance;
    }

    public float GetOptimalAltitude()
    {
        return requirement.OptimalAltitude;
    }

    public float GetTolerance()
    {
        return requirement.Tolerance;
    }

    public CropTreatment GetRequiredTreatment()
    {
        return requiredTreatment;
    }

    public void ReceiveTreatment(SprayType spray, float altitude)
    {
        if (!requirement.Needed)
        {
            return;
        }

        if ((CropTreatment)spray != requiredTreatment)
        {
            return;
        }

        if (!IsAltitudeCorrect(altitude))
        {
            return;
        }

        requirement.Needed = false;

        UpdateVisuals();

        GameEvents.OnPlantCompleted?.Invoke();

        MissionManager.Instance.NotifyCropCompleted(this);
    }

    public static void ResetAltitudes()
    {
        altitudesInitialized = false;
    }
}