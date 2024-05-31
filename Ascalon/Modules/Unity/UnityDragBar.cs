#if UNITY_2019_1_OR_NEWER
using UnityEngine;
using UnityEngine.EventSystems;

public class UnityDragBar : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    //storage
    private Vector2 dragPoint = Vector2.zero;


    [Header("Settings")]
    [SerializeField] private RectTransform dragRecipient;



    public void OnDrag(PointerEventData eventData)
    {
        dragRecipient.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - dragPoint;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragPoint = new Vector2(eventData.position.x, eventData.position.y) - dragRecipient.anchoredPosition;
    }
}
#endif
