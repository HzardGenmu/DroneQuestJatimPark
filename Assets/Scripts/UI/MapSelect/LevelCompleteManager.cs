using UnityEngine;

public enum LevelResult
{
    Complete,
    Failed
}

public class LevelCompleteManager : MonoBehaviour
{
    [Header("Current Level")]
    [SerializeField] private int levelIndex;

    [SerializeField] private DroneBattery battery;

    [Header("UI")]
    [SerializeField] private LevelResultPanel resultPanel;

    private bool levelFinished;

    public bool LevelFinished => levelFinished;

    //----------------------------------------
    // LEVEL COMPLETE
    //----------------------------------------

    public void CompleteLevel()
    {
        if (levelFinished)
            return;

        levelFinished = true;

        int stars = CalculateStars();

        MapSaveSystem.Instance.SaveStars(
            levelIndex,
            stars);

        MapSaveSystem.Instance.UnlockNextLevel(
            levelIndex);

        resultPanel.Show(
            LevelResult.Complete,
            stars);
    }

    //----------------------------------------
    // LEVEL FAILED
    //----------------------------------------

    public void FailLevel()
    {
        if (levelFinished)
            return;

        levelFinished = true;

        resultPanel.Show(
            LevelResult.Failed,
            0);
    }

    //----------------------------------------

    private int CalculateStars()
    {
        float batteryPercent =
            battery.CurrentBattery /
            battery.MaxBattery;

        if (batteryPercent >= .667f)
            return 3;

        if (batteryPercent >= .5f)
            return 2;

        if (batteryPercent >= .2f)
            return 1;

        return 0;
    }
}