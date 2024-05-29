#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Simple class to create draggable windows. Add a grab area as a panel, attach script to it,
//choose targeting mode.

public class UIDraggablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Vector3 dragOffset;

    public DragType dragMode = DragType.This;
    private RectTransform targetPanel;
    public GameObject targetObject;
    private RectTransform localBounds;

    public bool lockWindowToCanvas = true;

    [Tooltip("The Canvas this panel belongs to. Necessary to determine if this is screenspace or worldspace.")]
    public Canvas canvas;
    private bool canvasIsWorldSpace;

    private void Awake()
    {
        canvasIsWorldSpace = canvas.renderMode == RenderMode.WorldSpace;
        switch (dragMode)
        {
            case DragType.This:
                targetPanel = transform as RectTransform;
                break;

            case DragType.Parent:
                targetPanel = transform.parent as RectTransform;
                break;

            case DragType.Custom:
                targetPanel = targetObject.transform as RectTransform;
                break;
        }

        localBounds = GetComponent<RectTransform>();
    }

    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canvasIsWorldSpace)
        {
            targetPanel.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y) - dragOffset;
        }
        else
        {
            Vector3 newPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out newPosition);
            newPosition.z = 0;
            Debug.Log("hit " + newPosition.ToString());
            //targetPanel.localPosition = newPosition;
            targetPanel.localPosition = canvas.transform.TransformPoint(newPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canvasIsWorldSpace)
        {
            dragOffset = new Vector3(eventData.position.x, eventData.position.y) - targetPanel.transform.position;
        }
        else
        {
            Vector3 oldOffset = dragOffset;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out dragOffset);
            dragOffset -= targetPanel.transform.localPosition;
            Debug.Log("Offset altered: old is " + oldOffset.ToString() + " new is " + dragOffset.ToString());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        

        //todo: keep the drag bar within current screen bounds
    }

    public enum DragType
    {
        This,
        Parent,
        Custom
    }
}
#endif
