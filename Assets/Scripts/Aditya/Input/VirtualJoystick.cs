using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    [SerializeField] private RectTransform joystickArea;
    [SerializeField] private RectTransform joystickHandle;

    public Vector2 Direction { get; private set; }

    private float radius;

    private void Awake()
    {
        Direction = Vector2.zero;

        if (joystickArea != null)
        {
            radius = joystickArea.rect.width * 0.5f;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (joystickArea == null || joystickHandle == null || radius <= 0f)
        {
            return;
        }

        Vector2 localPosition;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickArea,
            eventData.position,
            eventData.pressEventCamera,
            out localPosition))
        {
            return;
        }

        Vector2 offset = localPosition;

        offset = Vector2.ClampMagnitude(offset, radius);

        Direction = offset / radius;

        joystickHandle.anchoredPosition = offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;

        if (joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }
}