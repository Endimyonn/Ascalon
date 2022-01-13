using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDraggablePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool dragging = false;

    private Vector3 dragOffset;

    public DragType dragMode = DragType.This;
    private Transform targetPanel;
    public GameObject targetObject;

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
        //Debug.Log("EventData: " + eventData.position.x + " | " + eventData.position.y + "\nInputMPOS: " + Input.mousePosition.x + " | " + Input.mousePosition.y);
        dragOffset = new Vector3(eventData.position.x, eventData.position.y) - transform.position;
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public enum DragType
    {
        This,
        Parent,
        Custom
    }
}

