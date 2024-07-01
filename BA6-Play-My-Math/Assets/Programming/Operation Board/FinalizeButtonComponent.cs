using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board {
    public class FinalizeButtonComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;
        public UnityEvent FinalizeEvent;
        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvas.worldCamera = Camera.main;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Button Down");
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Button Up");
            FinalizeEvent.Invoke();
        }
        
        
    }
}
