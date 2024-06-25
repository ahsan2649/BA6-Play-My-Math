using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OperatorWheelComponent : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
{
    RectTransform _rectTransform;
    Canvas _canvas;
    CanvasGroup _canvasGroup;
    private Vector2 dragStart, dragEnd;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
            
        _canvas.worldCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStart = eventData.position;
        Debug.Log("Drag Started!");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragEnd = eventData.position;
        Debug.Log("Drag Stopped");
        CalculateSwipe();
    }

    public void CalculateSwipe()
    {
        Vector2 direction = (dragEnd - dragStart).normalized;
        if (direction.y < -0.4f)
        {
            Debug.Log("Swipe down");
        }

        if (direction.y > 0.4f)
        {
            Debug.Log("Swipe Up");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging!");
    }
}
