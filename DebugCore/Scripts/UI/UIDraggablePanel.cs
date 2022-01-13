using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Simple class to create draggable windows. Add a grab area as a panel, attach script to it,
//choose targeting mode.

public class UIDraggablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool dragging = false;

    private Vector3 dragOffset;

    public DragType dragMode = DragType.This;
    private Transform targetPanel;
    public GameObject targetObject;
    private RectTransform localBounds;

    private void Awake()
    {
        switch (dragMode)
        {
            case DragType.This:
                targetPanel = transform;
                break;

            case DragType.Parent:
                targetPanel = transform.parent;
                break;

            case DragType.Custom:
                targetPanel = targetObject.transform;
                break;
        }

        localBounds = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (dragging)
        {
            targetPanel.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y) - dragOffset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragOffset = new Vector3(eventData.position.x, eventData.position.y) - transform.position;
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;

        //todo: keep the drag bar within current screen bounds
    }

    public enum DragType
    {
        This,
        Parent,
        Custom
    }
}

