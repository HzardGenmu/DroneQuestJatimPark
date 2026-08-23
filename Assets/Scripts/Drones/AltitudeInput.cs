using UnityEngine;

public class AltitudeInput : MonoBehaviour
{
    [SerializeField] private DroneController drone;

    [SerializeField]
    private float[] altitudeLevels =
    {
        2f,
        6f,
        10f,
        14f,
        18f
    };

    public void OnSliderChanged(float value)
    {
        int index = Mathf.RoundToInt(value);

        index = Mathf.Clamp(index, 0, altitudeLevels.Length - 1);

        drone.SetTargetAltitude(altitudeLevels[index]);
    }
}