using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("References")]
    [SerializeField] private MissionObjectiveUI objectiveUI;
    [SerializeField] private LevelCompleteManager levelCompleteManager;

    private readonly List<CropField> allCrops = new();

    private readonly List<CropField> waterCrops = new();
    private readonly List<CropField> fertilizerCrops = new();
    private readonly List<CropField> pesticideCrops = new();

    private int completedWater;
    private int completedFertilizer;
    private int completedPesticide;

    private bool missionCompleted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshUI();
    }

    public void RegisterCrop(CropField crop)
    {
        if (allCrops.Contains(crop))
            return;

        allCrops.Add(crop);

        switch (crop.GetRequiredTreatment())
        {
            case CropTreatment.Water:
                waterCrops.Add(crop);
                break;

            case CropTreatment.Fertilizer:
                fertilizerCrops.Add(crop);
                break;

            case CropTreatment.Pesticide:
                pesticideCrops.Add(crop);
                break;
        }

        RefreshUI();
    }

    public void NotifyCropCompleted(CropField crop)
    {
        if (missionCompleted)
            return;

        RefreshUI();

        if (completedWater +
            completedFertilizer +
            completedPesticide >= allCrops.Count)
        {
            missionCompleted = true;
            levelCompleteManager.CompleteLevel();
        }
    }

    private void RefreshUI()
    {
        if (objectiveUI == null)
            return;

        completedWater = waterCrops.Count(c => c.IsCompleted);
        completedFertilizer = fertilizerCrops.Count(c => c.IsCompleted);
        completedPesticide = pesticideCrops.Count(c => c.IsCompleted);

        objectiveUI.UpdateTreatmentProgress(
            CropTreatment.Water,
            completedWater,
            waterCrops.Count);

        objectiveUI.UpdateTreatmentProgress(
            CropTreatment.Fertilizer,
            completedFertilizer,
            fertilizerCrops.Count);

        objectiveUI.UpdateTreatmentProgress(
            CropTreatment.Pesticide,
            completedPesticide,
            pesticideCrops.Count);
    }

    #region Optional Public Getters

    public int TotalWater => waterCrops.Count;
    public int TotalFertilizer => fertilizerCrops.Count;
    public int TotalPesticide => pesticideCrops.Count;

    public int CompletedWater => completedWater;
    public int CompletedFertilizer => completedFertilizer;
    public int CompletedPesticide => completedPesticide;

    #endregion
}