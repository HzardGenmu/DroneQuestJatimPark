using UnityEngine;
using UnityEngine.InputSystem;

public class DroneBattery : MonoBehaviour
{
    [Header("Battery")]
    [SerializeField] private float maxBattery = 180f;

    [SerializeField] private float passiveDrainRate = 1f;

    [SerializeField] private float sprayDrainRate = 2f;

    [Header("Drone")]
    [SerializeField] private DroneController droneController;

    [Header("Managers")]
    [SerializeField] private LevelCompleteManager levelCompleteManager;

    [SerializeField] private PlayerInput playerInput;

    private float currentBattery;

    private bool batteryDepleted;

    private bool isSpraying;

    public float CurrentBattery => currentBattery;
    public float MaxBattery => maxBattery;

    private void Start()
    {
        currentBattery = maxBattery;
    }

    private void Update()
    {
        if (batteryDepleted)
            return;

        if (droneController.CurrentState != DroneController.DroneState.Flying)
            return;

        float drain = passiveDrainRate;

        if (isSpraying)
            drain += sprayDrainRate;

        currentBattery -= drain * Time.deltaTime;

        if (currentBattery <= 0f)
        {
            BatteryDepleted();
        }
    }

    private void BatteryDepleted()
    {
        batteryDepleted = true;

        currentBattery = 0f;

        playerInput.enabled = false;

        GameEvents.OnBatteryDepleted?.Invoke();

        Debug.Log("Battery Depleted");

        if (levelCompleteManager != null)
            levelCompleteManager.FailLevel();
    }

    public void SetSpraying(bool value)
    {
        isSpraying = value;
    }

    public float GetBatteryPercent()
    {
        return currentBattery / maxBattery;
    }
}