using UnityEngine;

public class AltitudeInput : MonoBehaviour
{
    [SerializeField] private DroneController drone;

    public void OnSliderChanged(float value)
    {
        drone.SetTargetAltitude(value);
    }
}