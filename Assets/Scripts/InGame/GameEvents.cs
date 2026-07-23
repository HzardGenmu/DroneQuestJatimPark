public static class GameEvents
{
    // Drone
    public static System.Action OnDroneTakeoff;
    public static System.Action OnDroneLanding;
    public static System.Action OnSprayStarted;
    public static System.Action OnSprayStopped;
    public static System.Action OnBatteryDepleted;

    // Plants
    public static System.Action OnPlantTreating;
    public static System.Action OnPlantCompleted;
    public static System.Action OnEveryPlantFinished;
    // Economy
    public static System.Action OnMoneyEarned;
    public static System.Action OnMoneySpent;

    // UI
    public static System.Action OnButtonPressed;
    public static System.Action OnLockedButton;

    // Timer
    public static System.Action OnTimerWarning;
    public static System.Action OnTimeout;
}