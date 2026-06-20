using UnityEngine;
using UnityEngine.InputSystem;

public class DroneBattery : MonoBehaviour
{
    [Header("Battery")]
    [SerializeField] private float maxBattery = 100f;

    [SerializeField] private float passiveDrainRate = 1f;

    [SerializeField] private float sprayDrainRate = 2f;

    [Header("UI")]
    [SerializeField] private GameObject batteryDepletedPanel;

    [SerializeField] private PlayerInput playerInput;

    private float currentBattery;

    private bool batteryDepleted;

    private bool isSpraying;

    private void Start()
    {
        currentBattery = maxBattery;

        batteryDepletedPanel.SetActive(false);
    }

    private void Update()
    {
        if (batteryDepleted)
            return;

        float drain = passiveDrainRate;

        if (isSpraying)
            drain += sprayDrainRate;

        currentBattery -= drain * Time.deltaTime;

        if (currentBattery <= 0)
        {
            BatteryDepleted();
        }
    }

    private void BatteryDepleted()
    {
        batteryDepleted = true;

        currentBattery = 0;

        batteryDepletedPanel.SetActive(true);

        playerInput.enabled = false;

        Debug.Log("Battery Depleted");
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