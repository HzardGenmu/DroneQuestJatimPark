using UnityEngine;
using System.Collections;
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
    private bool tutorialCameraTriggered;

    public bool IsBottomCameraActive => usingBottomCamera;

    private void Awake()
    {
        if (brain == null)
            brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    private void Start()
    {
        ShowFollowCamera();
    }

    //private void Update()
    //{
    //    if (usingBottomCamera)
    //        return;

    //    Vector2 look = cameraInput.GetLookDelta();

    //    orbitalFollow.HorizontalAxis.Value +=
    //        look.x * horizontalSensitivity;

    //    orbitalFollow.VerticalAxis.Value -=
    //        look.y * verticalSensitivity;
    //}

    private void Update()
    {
        if (usingBottomCamera)
            return;

        if (!cameraInput.HasCameraFinger)
            return;

        Vector2 look = cameraInput.LookDelta;

        orbitalFollow.HorizontalAxis.Value += look.x * horizontalSensitivity;
        //orbitalFollow.VerticalAxis.Value -= look.y * verticalSensitivity;
    }

    public void ToggleCamera()
    {
        if (usingBottomCamera)
            ShowFollowCamera();
        else
            ShowBottomCamera();

        if (!tutorialCameraTriggered)
            StartCoroutine(WaitForBlend());
    }

    private IEnumerator WaitForBlend()
    {
        // Wait until a blend starts
        while (!brain.IsBlending)
            yield return null;

        // Wait until it finishes
        while (brain.IsBlending)
            yield return null;

        tutorialCameraTriggered = true;
        GameEvents.OnCameraModeChanged?.Invoke();
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
        usingBottomCamera = false;

        followCamera.Priority = activePriority;
        bottomCamera.Priority = inactivePriority;

        if (crosshair != null)
            crosshair.SetActive(false);
    }
}