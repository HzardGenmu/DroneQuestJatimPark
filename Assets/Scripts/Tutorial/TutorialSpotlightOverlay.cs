using UnityEngine;
using UnityEngine.UI;

public class TutorialSpotlightOverlay : MonoBehaviour
{
    [SerializeField]
    private Image overlayImage;

    [SerializeField]
    private Camera worldCamera;

    private Material runtimeMaterial;

    private readonly Vector4[] holes =
        new Vector4[4];

    private readonly Transform[] trackedTargets =
        new Transform[4];

    private readonly Vector2[] trackedSizes =
        new Vector2[4];

    private readonly Vector2[] trackedOffsets =
        new Vector2[4];

    private readonly bool[] useAutoSize =
        new bool[4];

    private void Awake()
    {
        runtimeMaterial =
            Instantiate(overlayImage.material);

        overlayImage.material =
            runtimeMaterial;

        if (worldCamera == null)
            worldCamera = Camera.main;

        ClearHoles();
    }

    private void Update()
    {
        RectTransform overlayRect =
            overlayImage.rectTransform;

        Rect overlayBounds =
            overlayRect.rect;

        bool changed = false;

        for (int i = 0; i < trackedTargets.Length; i++)
        {
            Transform target =
                trackedTargets[i];

            if (target == null)
                continue;

            Vector2 center;
            Vector2 size;

            // -------------------------
            // UI OBJECTS
            // -------------------------
            if (target is RectTransform rect)
            {
                if (!TryGetUIRect(
                        rect,
                        overlayRect,
                        out center,
                        out size))
                {
                    continue;
                }

                if (!useAutoSize[i])
                {
                    size =
                        trackedSizes[i];
                }
            }
            // -------------------------
            // WORLD OBJECTS
            // -------------------------
            else
            {
                if (!TryGetWorldPosition(
                        target,
                        overlayRect,
                        out center))
                {
                    continue;
                }

                if (useAutoSize[i])
                {
                    Renderer renderer =
                        target.GetComponentInChildren<Renderer>();

                    if (renderer != null)
                    {
                        Vector3 screenMin =
                            worldCamera.WorldToScreenPoint(
                                renderer.bounds.min
                            );

                        Vector3 screenMax =
                            worldCamera.WorldToScreenPoint(
                                renderer.bounds.max
                            );

                        size =
                            new Vector2(
                                Mathf.Abs(
                                    screenMax.x -
                                    screenMin.x
                                ),
                                Mathf.Abs(
                                    screenMax.y -
                                    screenMin.y
                                )
                            );
                    }
                    else
                    {
                        size =
                            trackedSizes[i];
                    }
                }
                else
                {
                    size =
                        trackedSizes[i];
                }
            }

            center += trackedOffsets[i];

            float normalizedX =
                (center.x - overlayBounds.xMin)
                / overlayBounds.width;

            float normalizedY =
                (center.y - overlayBounds.yMin)
                / overlayBounds.height;

            float normalizedWidth =
                size.x / overlayBounds.width;

            float normalizedHeight =
                size.y / overlayBounds.height;

            holes[i] =
                new Vector4(
                    normalizedX,
                    normalizedY,
                    normalizedWidth,
                    normalizedHeight
                );

            changed = true;
        }

        if (changed)
            Apply();
    }

    bool TryGetUIRect(
        RectTransform target,
        RectTransform overlayRect,
        out Vector2 center,
        out Vector2 size)
    {
        center = Vector2.zero;
        size = Vector2.zero;

        Vector3[] corners =
            new Vector3[4];

        target.GetWorldCorners(corners);

        Vector2 min =
            new Vector2(
                float.MaxValue,
                float.MaxValue
            );

        Vector2 max =
            new Vector2(
                float.MinValue,
                float.MinValue
            );

        for (int i = 0; i < 4; i++)
        {
            Vector2 localPoint;

            RectTransformUtility
                .ScreenPointToLocalPointInRectangle(
                    overlayRect,
                    RectTransformUtility
                        .WorldToScreenPoint(
                            null,
                            corners[i]
                        ),
                    null,
                    out localPoint
                );

            min =
                Vector2.Min(
                    min,
                    localPoint
                );

            max =
                Vector2.Max(
                    max,
                    localPoint
                );
        }

        center =
            (min + max) * 0.5f;

        size =
            max - min;

        return true;
    }

    bool TryGetWorldPosition(
        Transform target,
        RectTransform overlayRect,
        out Vector2 center)
    {
        center = Vector2.zero;

        if (worldCamera == null)
            return false;

        Vector3 screenPos =
            worldCamera.WorldToScreenPoint(
                target.position
            );

        if (screenPos.z < 0)
            return false;

        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                overlayRect,
                screenPos,
                null,
                out center
            );

        return true;
    }

    public void TrackTarget(
        int index,
        Transform target,
        Vector2 size,
        bool autoSize,
        Vector2 offset
    )
    {
        if (
            index < 0 ||
            index >= trackedTargets.Length
        )
        {
            return;
        }

        trackedTargets[index] =
            target;

        trackedSizes[index] =
            size;

        trackedOffsets[index] =
            offset;

        useAutoSize[index] =
            autoSize;
    }

    public void ClearTarget(int index)
    {
        if (
            index < 0 ||
            index >= trackedTargets.Length
        )
        {
            return;
        }

        trackedTargets[index] =
            null;

        trackedSizes[index] =
            Vector2.zero;

        trackedOffsets[index] =
            Vector2.zero;

        useAutoSize[index] =
            false;

        holes[index] =
            new Vector4(
                -1,
                -1,
                0,
                0
            );

        Apply();
    }

    public void ClearHoles()
    {
        for (int i = 0; i < holes.Length; i++)
        {
            trackedTargets[i] =
                null;

            trackedSizes[i] =
                Vector2.zero;

            trackedOffsets[i] =
                Vector2.zero;

            useAutoSize[i] =
                false;

            holes[i] =
                new Vector4(
                    -1,
                    -1,
                    0,
                    0
                );
        }

        Apply();
    }

    //void Apply()
    //{
    //    if (runtimeMaterial == null)
    //        return;

    //    runtimeMaterial.SetVector(
    //        "_Hole1",
    //        holes[0]);

    //    runtimeMaterial.SetVector(
    //        "_Hole2",
    //        holes[1]);

    //    runtimeMaterial.SetVector(
    //        "_Hole3",
    //        holes[2]);

    //    runtimeMaterial.SetVector(
    //        "_Hole4",
    //        holes[3]);
    //}

    void Apply()
    {
        Material mat =
            overlayImage.material;

        mat.SetVector("_Hole1", holes[0]);
        mat.SetVector("_Hole2", holes[1]);
        mat.SetVector("_Hole3", holes[2]);
        mat.SetVector("_Hole4", holes[3]);
    }
}