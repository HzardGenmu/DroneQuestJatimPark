using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class DroneCamera : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private CinemachineCamera bottomCamera;
    [SerializeField] private GameObject crosshair;

    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;

    private bool usingBottomCamera;

    private void Start()
    {
        usingBottomCamera = false;

        followCamera.Priority = activePriority;
        bottomCamera.Priority = inactivePriority;

        if (crosshair != null)
            crosshair.SetActive(false);
    }

    public void ToggleCamera()
    {
        usingBottomCamera = !usingBottomCamera;

        followCamera.Priority =
            usingBottomCamera ? inactivePriority : activePriority;

        bottomCamera.Priority =
            usingBottomCamera ? activePriority : inactivePriority;

        if (crosshair != null)
            crosshair.SetActive(usingBottomCamera);
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

    public bool IsBottomCameraActive => usingBottomCamera;
}