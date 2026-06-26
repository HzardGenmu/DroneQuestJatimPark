using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DroneViewer :
    MonoBehaviour,
    IDragHandler,
    IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private Transform dronePivot;
    [SerializeField] private Camera viewerCamera;
    [SerializeField] private DroneDisplayManager displayManager;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float smoothness = 10f;

    [Header("Zoom")]
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float zoomSpeed = 0.01f;

    [Header("Idle")]
    [SerializeField] private float idleDelay = 1f;

    private float targetRotationY;

    private float idleTimer;

    private Vector3 cameraStartPosition;

    private void Start()
    {
        cameraStartPosition =
            viewerCamera.transform.localPosition;

        targetRotationY =
            dronePivot.eulerAngles.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        idleTimer = 0f;
        displayManager.CurrentDrone.GetComponent<DroneIdle>().enabled = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        idleTimer = 0f;

        targetRotationY +=
            -eventData.delta.x *
            rotationSpeed;
    }

    void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDelay)
        {
            displayManager.CurrentDrone.GetComponent<DroneIdle>().enabled = true;
        }

        Vector3 rot =
            dronePivot.eulerAngles;

        float y =
            Mathf.LerpAngle(
                rot.y,
                targetRotationY,
                smoothness * Time.deltaTime);

        dronePivot.rotation =
            Quaternion.Euler(0, y, 0);

        HandleZoom();

        HandleDoubleTap();
    }

    private void HandleZoom()
    {
        if (Touchscreen.current == null)
            return;

        if (Touchscreen.current.touches.Count < 2)
            return;

        var touch0 =
            Touchscreen.current.touches[0];

        var touch1 =
            Touchscreen.current.touches[1];

        if (!touch0.press.isPressed ||
            !touch1.press.isPressed)
            return;

        float previousDistance =
            Vector2.Distance(
                touch0.position.ReadValue() - touch0.delta.ReadValue(),
                touch1.position.ReadValue() - touch1.delta.ReadValue());

        float currentDistance =
            Vector2.Distance(
                touch0.position.ReadValue(),
                touch1.position.ReadValue());

        float delta =
            currentDistance - previousDistance;

        Vector3 pos =
            viewerCamera.transform.localPosition;

        pos.z += delta * zoomSpeed;

        pos.z = Mathf.Clamp(
            pos.z,
            -maxDistance,
            -minDistance);

        viewerCamera.transform.localPosition = pos;

        idleTimer = 0f;
        displayManager.CurrentDrone.GetComponent<DroneIdle>().enabled = false;
    }

    private float lastTapTime;

    private void HandleDoubleTap()
    {
        if (Touchscreen.current == null)
            return;

        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return;

        if (Time.time - lastTapTime < 0.3f)
        {
            ResetView();
        }

        lastTapTime = Time.time;
    }

    public void ResetView()
    {
        targetRotationY = 0;

        viewerCamera.transform.localPosition =
            cameraStartPosition;

        idleTimer = 0f;
    }
}