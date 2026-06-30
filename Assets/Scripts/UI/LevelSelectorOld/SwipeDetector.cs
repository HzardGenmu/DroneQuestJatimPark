using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeDetector :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [SerializeField]
    private LevelSelectManager manager;

    private Vector2 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = eventData.position;

        manager.BeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        manager.Drag(eventData.delta.x);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 delta =
            eventData.position - startPosition;

        manager.EndDrag(delta.x);
    }
}