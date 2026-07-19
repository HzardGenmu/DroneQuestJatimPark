using UnityEngine;

[System.Serializable]
public class TreatmentRequirement
{
    public bool Needed;

    [Range(2, 20)]
    public float OptimalAltitude = 5f;

    [Range(0.1f, 3f)]
    public float Tolerance = 0.5f;
}