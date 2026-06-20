using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private DroneBattery battery;

    [SerializeField] private Slider batterySlider;

    private void Update()
    {
        batterySlider.value =
            battery.GetBatteryPercent();
    }
}