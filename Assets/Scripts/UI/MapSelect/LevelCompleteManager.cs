using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("Current Level")]
    [SerializeField] private int levelIndex;

    [SerializeField] private DroneBattery battery;

    [SerializeField] private string mapScene = "MapSelector";

    [SerializeField] private CongratulationsPanel congratulationsPanel;
    public void CompleteLevel()
    {
        int stars = CalculateStars();

        MapSaveSystem.Instance.SaveStars(
            levelIndex,
            stars);

        MapSaveSystem.Instance.UnlockNextLevel(
            levelIndex);

        congratulationsPanel.Show();
    }


    private int CalculateStars()
    {
        float batteryPercent =
            battery.CurrentBattery /
            battery.MaxBattery;

        if (batteryPercent >= 0.8f)
            return 3;

        if (batteryPercent >= 0.5f)
            return 2;

        if (batteryPercent >= 0.2f)
            return 1;

        return 0;
    }
}