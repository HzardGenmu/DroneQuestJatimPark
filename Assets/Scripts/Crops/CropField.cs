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

    public bool IsCompleted => !requirement.Needed;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;

        requiredTreatment = (CropTreatment)Random.Range(0, 3);

        requirement.Needed = true;
        requirement.OptimalAltitude = Random.Range(2f, 20f);

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
        Debug.Log($"[{name}] ReceiveTreatment called.");
        Debug.Log($"[{name}] Required: {requiredTreatment}");
        Debug.Log($"[{name}] Spray: {spray}");

        if (!requirement.Needed)
        {
            Debug.Log($"[{name}] Already completed.");
            return;
        }

        if ((CropTreatment)spray != requiredTreatment)
        {
            Debug.Log($"[{name}] Wrong treatment type.");
            return;
        }

        if (!IsAltitudeCorrect(altitude))
        {
            Debug.Log($"[{name}] Incorrect altitude.");
            Debug.Log($"Optimal: {requirement.OptimalAltitude:F2}");
            Debug.Log($"Current: {altitude:F2}");
            return;
        }

        Debug.Log($"[{name}] Treatment SUCCESS!");

        requirement.Needed = false;

        UpdateVisuals();

        GameEvents.OnPlantCompleted?.Invoke();

        MissionManager.Instance.NotifyCropCompleted(this);
    }
}