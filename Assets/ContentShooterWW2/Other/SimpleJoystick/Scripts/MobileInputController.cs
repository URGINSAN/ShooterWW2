using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(UnityEngine.UI.AspectRatioFitter))]
public class MobileInputController : MonoBehaviour,IEndDragHandler,IDragHandler,IPointerDownHandler,IPointerUpHandler {

    public RectTransform Background;
    public RectTransform Knob;
    [Header("Input Values")]
    public float Horizontal = 0;
    public float Vertical = 0;

    public float offset;
    Vector2 PointPosition;
    
    public void OnDrag(PointerEventData eventData)
    {
        PointPosition = new Vector2((eventData.position.x - Background.position.x) / ((Background.rect.size.x - Knob.rect.size.x) / 2), (eventData.position.y - Background.position.y) / ((Background.rect.size.y - Knob.rect.size.y) / 2));
        PointPosition = (PointPosition.magnitude>1.0f)?PointPosition.normalized :PointPosition;
        Knob.transform.position = new Vector2((PointPosition.x *((Background.rect.size.x-Knob.rect.size.x)/2)*offset) + Background.position.x, (PointPosition.y* ((Background.rect.size.y-Knob.rect.size.y)/2) *offset) + Background.position.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PointPosition = new Vector2(0f,0f);
        Knob.transform.position = Background.position;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        OnEndDrag(eventData);
    }

    void Update()
    {
        Horizontal = PointPosition.x;
        Vertical = PointPosition.y;
        /*
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //knowing the touch position
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

            //touch phase

            switch (touch.phase)
            {
                case TouchPhase.Moved:
                    PointPosition = new Vector2((touchPos.x - Background.position.x) / ((Background.rect.size.x - Knob.rect.size.x) / 2), (touchPos.y - Background.position.y) / ((Background.rect.size.y - Knob.rect.size.y) / 2));
                    PointPosition = (PointPosition.magnitude > 1.0f) ? PointPosition.normalized : PointPosition;
                    Knob.transform.position = new Vector2((PointPosition.x * ((Background.rect.size.x - Knob.rect.size.x) / 2) * offset) + Background.position.x, (PointPosition.y * ((Background.rect.size.y - Knob.rect.size.y) / 2) * offset) + Background.position.y);

                    break;
            }
        }*/
    }

    public Vector2 Coordinate()
    {
        return new Vector2(Horizontal,Vertical);
    }
}