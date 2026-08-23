using UnityEngine;
using UnityEngine.EventSystems;

public class DroneCameraInput :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public Vector2 LookDelta { get; private set; }

    public bool HasCameraFinger { get; private set; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        HasCameraFinger = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        LookDelta = eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        HasCameraFinger = false;
        LookDelta = Vector2.zero;
    }

    private void LateUpdate()
    {
        LookDelta = Vector2.zero;
    }
}