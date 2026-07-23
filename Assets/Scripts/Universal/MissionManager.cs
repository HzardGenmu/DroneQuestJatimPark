using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("References")]
    [SerializeField] private MissionObjectiveUI objectiveUI;
    [SerializeField] private LevelCompleteManager levelCompleteManager;

    private readonly List<CropField> cropFields = new();

    private int totalFields;
    private int completedFields;

    private bool missionCompleted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        completedFields = 0;

        UpdateObjectiveUI();
    }

    public void NotifyCropCompleted(CropField crop)
    {
        if (missionCompleted)
            return;

        completedFields++;

        UpdateObjectiveUI();

        Debug.Log($"Crop completed: {crop.name} ({crop.GetEntityId()}) - Progress: {completedFields}/{totalFields}");
        if (completedFields >= totalFields)
        {
            missionCompleted = true;
            Debug.Log("All crops completed! Mission complete.");
            levelCompleteManager.CompleteLevel();
            Debug.Log("Level complete UI triggered.");
        }
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveUI == null)
            return;

        string objective =
            GameManager.Instance.CurrentLevel.objective;

        objective +=
            $"\n\nProgress: {completedFields}/{totalFields}";

        objectiveUI.SetObjective(objective);
    }

    public void RegisterCrop(CropField crop)
    {
        if (cropFields.Contains(crop))
        {
            Debug.Log($"Duplicate registration ignored: {crop.name} ({crop.GetEntityId()})");
            return;
        }

        cropFields.Add(crop);
        totalFields = cropFields.Count;

        Debug.Log($"Registered #{totalFields}: {crop.name} ({crop.GetEntityId()})");

        UpdateObjectiveUI();
    }
}