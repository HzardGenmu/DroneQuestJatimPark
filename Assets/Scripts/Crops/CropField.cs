using UnityEngine;

public class CropField : MonoBehaviour
{
    [Header("Needs")]
    [SerializeField] private bool needsWater;
    [SerializeField] private bool needsFertilizer;
    [SerializeField] private bool needsPesticide;

    private Renderer rend;

    private bool scanned;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Start()
    {
        needsWater = Random.value > 0.5f;
        needsFertilizer = Random.value > 0.5f;
        needsPesticide = Random.value > 0.5f;

        UpdateVisuals();
    }

    public void SetScanned(bool value)
    {
        scanned = value;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (!scanned)
        {
            rend.material.color = Color.white;
            return;
        }

        if (needsPesticide)
        {
            rend.material.color = Color.red;
        }
        else if (needsFertilizer)
        {
            rend.material.color = Color.yellow;
        }
        else if (needsWater)
        {
            rend.material.color = Color.green;
        }
        else
        {
            rend.material.color = Color.white;
        }
    }

    public void ReceiveTreatment(SprayType sprayType)
    {
        switch (sprayType)
        {
            case SprayType.Water:
                needsWater = false;
                break;

            case SprayType.Fertilizer:
                needsFertilizer = false;
                break;

            case SprayType.Pesticide:
                needsPesticide = false;
                break;
        }

        UpdateVisuals();
    }
}