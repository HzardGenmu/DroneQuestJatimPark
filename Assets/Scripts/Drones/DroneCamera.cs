using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class DroneCamera : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private CinemachineCamera bottomCamera;
    [SerializeField] private CinemachineBrain brain;

    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private DroneCameraInput cameraInput;

    [SerializeField] private GameObject crosshair;

    [Header("Look")]
    [SerializeField] private float horizontalSensitivity = 0.05f;
    [SerializeField] private float verticalSensitivity = 0.05f;

    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;

    private bool usingBottomCamera;

    public bool IsBottomCameraActive => usingBottomCamera;

    private void Awake()
    {
        if (brain == null)
            brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    private void Start()
    {
        usingBottomCamera = false;

        followCamera.Priority = activePriority;
        bottomCamera.Priority = inactivePriority;

        if (crosshair != null)
            crosshair.SetActive(false);

        // Initial state
        GameEvents.SetCameraMode(false);
    }

    private void Update()
    {
        if (usingBottomCamera)
            return;

        if (!cameraInput.HasCameraFinger)
            return;

        Vector2 look = cameraInput.LookDelta;

        orbitalFollow.HorizontalAxis.Value +=
            look.x * horizontalSensitivity;
    }

    public void ToggleCamera()
    {
        if (usingBottomCamera)
        {
            ShowFollowCamera();
        }
        else
        {
            ShowBottomCamera();
            StartCoroutine(WaitForBottomCamera());
        }
    }

    private IEnumerator WaitForBottomCamera()
    {
        // Give Cinemachine one frame to process the priority change.
        yield return null;

        // If a blend is happening, wait until it is completely finished.
        while (brain != null && brain.IsBlending)
        {
            yield return null;
        }

        // The camera is now actually in bottom-camera mode.
        GameEvents.SetCameraMode(true);
    }

    public void ShowBottomCamera()
    {
        usingBottomCamera = true;

        followCamera.Priority = inactivePriority;
        bottomCamera.Priority = activePriority;

        if (crosshair != null)
            crosshair.SetActive(true);
    }

    public void ShowFollowCamera()
    {
        bool wasBottomCamera = usingBottomCamera;

        usingBottomCamera = false;

        followCamera.Priority = activePriority;
        bottomCamera.Priority = inactivePriority;

        if (crosshair != null)
            crosshair.SetActive(false);

        if (wasBottomCamera)
        {
            GameEvents.SetCameraMode(false);
        }
    }
}