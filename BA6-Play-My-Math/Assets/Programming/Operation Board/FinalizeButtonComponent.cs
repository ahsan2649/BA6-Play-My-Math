using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board {
    public class FinalizeButtonComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;
        

        private void OnEnable()
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

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Button Down");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Button Up");

        }
    }
}
