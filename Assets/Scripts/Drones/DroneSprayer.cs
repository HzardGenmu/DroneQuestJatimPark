using UnityEngine;

public enum SprayType
{
    Water,
    Fertilizer,
    Pesticide
}

public class DroneSprayer : MonoBehaviour
{
    [SerializeField] private Color waterColor = Color.cyan;
    [SerializeField] private Color fertilizerColor = Color.yellow;
    [SerializeField] private Color pesticideColor = Color.red;

    [SerializeField] private Transform sprayOrigin;
    [SerializeField] private float sprayRadius = 2f;
    [SerializeField] private ParticleSystem sprayParticles;

    private bool isSpraying;

    [SerializeField] private float sprayInterval = 0.2f;

    private float sprayTimer;

    [SerializeField] private DroneBattery battery;

    private SprayType currentSprayType;

    private void Update()
    {
        if (!isSpraying)
            return;

        sprayTimer += Time.deltaTime;

        if (sprayTimer >= sprayInterval)
        {
            sprayTimer = 0f;
            ApplySpray();
        }
    }

    public void SelectWater()
    {
        currentSprayType = SprayType.Water;

        UpdateSprayColor();
        Debug.Log("Selected Water");
    }

    public void SelectFertilizer()
    {
        currentSprayType = SprayType.Fertilizer;

        UpdateSprayColor();
        Debug.Log("Selected Fertilizer");
    }

    public void SelectPesticide()
    {
        currentSprayType = SprayType.Pesticide;

        UpdateSprayColor();
        Debug.Log("Selected Pesticide");
    }

    public void StartSpraying()
    {
        if (isSpraying)
            return;

        isSpraying = true;

        Debug.Log($"START SPRAYING {currentSprayType}");

        sprayParticles.Play();
        battery.SetSpraying(true);
    }

    public void StopSpraying()
    {
        if (!isSpraying)
            return;

        isSpraying = false;

        Debug.Log("STOP SPRAYING");

        sprayParticles.Stop();
        battery.SetSpraying(false);
    }

    private void ApplySpray()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                sprayOrigin.position,
                sprayRadius);

        foreach (Collider hit in hits)
        {
            CropField crop =
                hit.GetComponentInParent<CropField>();

            if (crop == null)
                continue;

            crop.ReceiveTreatment(currentSprayType);
        }
    }

    private void UpdateSprayColor()
    {
        var main = sprayParticles.main;

        switch (currentSprayType)
        {
            case SprayType.Water:
                main.startColor = waterColor;
                break;

            case SprayType.Fertilizer:
                main.startColor = fertilizerColor;
                break;

            case SprayType.Pesticide:
                main.startColor = pesticideColor;
                break;
        }
    }
}