using UnityEngine;

public class AltitudeInput : MonoBehaviour
{
    public DroneController drone;

    public void OnSliderChanged(float value)
    {
        drone.SetTargetAltitude(value);
    }
}
