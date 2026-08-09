using UnityEngine;

public class FloatingJoystick : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas canvas;

    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float movementRange = 100f;
    [SerializeField] private float deadZone = 0.15f;

    public Vector2 Input { get; private set; }

    public bool IsDragging => dragging;

    private bool dragging;
    private int pointerId = -1;

    private Vector2 startPosition;

    private Vector2 handleStartPosition;

    private void Awake()
    {
        // root.gameObject.SetActive(false);

        handleStartPosition = handle.anchoredPosition;
    }

    public void Begin(Vector2 screenPosition, int fingerId)
    {
        if (dragging)
            return;

        dragging = true;
        pointerId = fingerId;

        // root.gameObject.SetActive(true);

        RectTransform parentRect =
            root.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition,
            null,
            out startPosition);

        // root.anchoredPosition = startPosition;

        // background.anchoredPosition = Vector2.zero;
        // handle.anchoredPosition = Vector2.zero;

        Input = Vector2.zero;
    }

    public void Drag(Vector2 screenPosition, int fingerId)
    {
        if (!dragging || fingerId != pointerId)
            return;

        Vector2 currentPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            root.parent as RectTransform,
            screenPosition,
            null,
            out currentPosition);

        Vector2 delta =
            currentPosition - startPosition;

        Vector2 clamped =
            Vector2.ClampMagnitude(
                delta,
                movementRange);

        handle.anchoredPosition =
            handleStartPosition + clamped;

        Input =
            clamped / movementRange;

        if (Input.magnitude < deadZone)
            Input = Vector2.zero;
    }

    public void End(int fingerId)
    {
        if (fingerId != pointerId)
            return;

        dragging = false;
        pointerId = -1;

        Input = Vector2.zero;

        handle.anchoredPosition =
            handleStartPosition;

        // root.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        dragging = false;
        pointerId = -1;

        Input = Vector2.zero;

        handle.anchoredPosition =
            handleStartPosition;

        // root.gameObject.SetActive(false);
    }
}