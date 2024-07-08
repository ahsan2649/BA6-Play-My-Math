using System;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Programming.Operation_Board {
    public class FinalizeButtonComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public OperationBoardComponent operationBoardComponent; 
        
        Canvas _canvas; 
        
        public UnityEvent FinalizeEvent;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            
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
