using UnityEngine;

public class DroneActionManager : MonoBehaviour
{
    [SerializeField] private DroneController drone;
    [SerializeField] private DroneSprayer sprayer;

    public enum ActionMode
    {
        Spray,
        Land,
        Takeoff
    }

    public ActionMode CurrentMode { get; private set; }

    private void Update()
    {
        UpdateActionMode();
    }

    private void UpdateActionMode()
    {
        switch (drone.CurrentState)
        {
            case DroneController.DroneState.Landed:
                CurrentMode = ActionMode.Takeoff;
                return;

            case DroneController.DroneState.TakingOff:
                CurrentMode = ActionMode.Takeoff;
                return;

            case DroneController.DroneState.Landing:
                CurrentMode = ActionMode.Land;
                return;
        }

        if (drone.CanLand())
        {
            CurrentMode = ActionMode.Land;
        }
        else
        {
            CurrentMode = ActionMode.Spray;
        }
    }

    public void OnActionPressed()
    {
        switch (CurrentMode)
        {
            case ActionMode.Spray:
                sprayer.StartSpraying();
                break;

            case ActionMode.Land:
                drone.BeginLanding();
                break;

            case ActionMode.Takeoff:
                drone.BeginTakeoff();
                break;
        }
    }

    public void OnActionReleased()
    {
        if (CurrentMode ==
            ActionMode.Spray)
        {
            sprayer.StopSpraying();
        }
    }
}