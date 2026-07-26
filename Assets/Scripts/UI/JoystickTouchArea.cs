using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickTouchArea :
    MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [SerializeField]
    private FloatingJoystick joystick;

    public void OnPointerDown(PointerEventData eventData)
    {
        joystick.Begin(
            eventData.position,
            eventData.pointerId);
    }

    public void OnDrag(PointerEventData eventData)
    {
        joystick.Drag(
            eventData.position,
            eventData.pointerId);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystick.End(
            eventData.pointerId);
    }
}