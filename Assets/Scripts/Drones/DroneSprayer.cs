using System.Collections.Generic;
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

    [SerializeField] private Transform[] sprayOrigins;
    [SerializeField] private float sprayRadius = 1f;
    [SerializeField] private float sprayLength = 5f;

    private ParticleSystem[] sprayParticles;
    private bool isSpraying;

    [SerializeField] private float sprayInterval = 0.2f;

    private float sprayTimer;

    [SerializeField] private DroneBattery battery;
    [SerializeField] private DroneController drone;

    private SprayType currentSprayType;
    public SprayType CurrentSprayType => currentSprayType;

    private void Awake()
    {
        sprayParticles = new ParticleSystem[sprayOrigins.Length];

        for (int i = 0; i < sprayOrigins.Length; i++)
        {
            if (sprayOrigins[i] != null)
            {
                sprayParticles[i] =
                    sprayOrigins[i].GetComponentInChildren<ParticleSystem>();

                if (sprayParticles[i] == null)
                {
                    Debug.LogWarning(
                        $"No ParticleSystem found under {sprayOrigins[i].name}");
                }
            }
        }
    }

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
        if (drone.CurrentState != DroneController.DroneState.Flying)
            return;

        isSpraying = true;
        GameEvents.OnSprayStarted?.Invoke();
        
        foreach (ParticleSystem ps in sprayParticles)
        {
            if (ps != null)
                ps.Play();
        }

        battery.SetSpraying(true);
    }

    public void StopSpraying()
    {
        if (!isSpraying)
            return;

        isSpraying = false;

        GameEvents.OnSprayStopped?.Invoke();

        Debug.Log("STOP SPRAYING");

        foreach (ParticleSystem ps in sprayParticles)
        {
            if (ps != null)
                ps.Stop();
        }

        battery.SetSpraying(false);
    }

    private void ApplySpray()
    {
        Debug.Log($"[SPRAY] Applying {currentSprayType} at altitude {drone.CurrentAltitude:F2}");

        HashSet<CropField> treatedCrops = new();

        foreach (Transform origin in sprayOrigins)
        {
            if (origin == null)
                continue;

            RaycastHit[] hits = Physics.SphereCastAll(
                origin.position,
                sprayRadius,
                -origin.up,
                sprayLength);

            Debug.Log($"[SPRAY] {origin.name} hit {hits.Length} colliders.");

            foreach (RaycastHit hit in hits)
            {
                Debug.Log($"[SPRAY] Collider: {hit.collider.name}");

                CropField crop = hit.collider.GetComponentInParent<CropField>();

                if (crop == null)
                {
                    Debug.Log("[SPRAY] -> No CropField found.");
                    continue;
                }

                // Prevent treating the same crop multiple times
                if (!treatedCrops.Add(crop))
                    continue;

                Debug.Log($"[SPRAY] -> Found CropField: {crop.name}");

                crop.ReceiveTreatment(
                    currentSprayType,
                    drone.CurrentAltitude);
            }
        }
    }

    private void UpdateSprayColor()
    {
        foreach (ParticleSystem ps in sprayParticles)
        {
            if (ps == null)
                continue;

            var main = ps.main;

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
    private void OnDrawGizmosSelected()
    {
        if (sprayOrigins == null)
            return;

        Gizmos.color = Color.cyan;

        foreach (Transform origin in sprayOrigins)
        {
            if (origin == null)
                continue;

            Vector3 start = origin.position;
            Vector3 end = start + (-origin.up * sprayLength);

            Gizmos.DrawLine(start, end);

            DrawWireCircle(start, sprayRadius, origin);
            DrawWireCircle(end, sprayRadius, origin);

            Gizmos.DrawLine(start + origin.right * sprayRadius,
                            end + origin.right * sprayRadius);

            Gizmos.DrawLine(start - origin.right * sprayRadius,
                            end - origin.right * sprayRadius);

            Gizmos.DrawLine(start + origin.forward * sprayRadius,
                            end + origin.forward * sprayRadius);

            Gizmos.DrawLine(start - origin.forward * sprayRadius,
                            end - origin.forward * sprayRadius);
        }
    }

    private void DrawWireCircle(Vector3 center, float radius, Transform origin)
    {
        const int segments = 32;

        Vector3 prev = center + origin.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 next =
                center +
                (origin.right * Mathf.Cos(angle) +
                 origin.forward * Mathf.Sin(angle)) * radius;

            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}