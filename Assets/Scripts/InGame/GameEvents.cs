using System;

public static class GameEvents
{
    // Drone
    public static Action OnDroneTakeoff;
    public static Action OnDroneLanding;
    public static Action OnSprayStarted;
    public static Action OnSprayStopped;
    public static Action OnBatteryDepleted;
    public static Action OnCameraModeChanged;

    // Current camera state
    public static bool IsBottomCameraActive { get; private set; }

    public static void SetCameraMode(bool bottomCameraActive)
    {
        if (IsBottomCameraActive == bottomCameraActive)
            return;

        IsBottomCameraActive = bottomCameraActive;

        OnCameraModeChanged?.Invoke();
    }

    // Plants
    public static Action OnPlantTreating;
    public static Action OnPlantCompleted;
    public static Action OnEveryPlantFinished;
    public static Action<CropField> OnPlantScanned;
    public static Action<CropField> OnPlantTreated;

    // Economy
    public static Action OnMoneyEarned;
    public static Action OnMoneySpent;

    // UI
    public static Action OnButtonPressed;
    public static Action OnLockedButton;

    // Timer
    public static Action OnTimerWarning;
    public static Action OnTimeout;

    // Tutorial
    public static Action<TutorialTriggerType> OnTutorialStepStarted;
}