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

        if (completedFields >= totalFields)
        {
            missionCompleted = true;

            levelCompleteManager.CompleteLevel();
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
        if (!cropFields.Contains(crop))
        {
            cropFields.Add(crop);
            totalFields = cropFields.Count;

            UpdateObjectiveUI();
        }
    }
}