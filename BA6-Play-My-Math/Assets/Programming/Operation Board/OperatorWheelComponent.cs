using System;
using System.Collections;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Programming.Operation_Board
{
    public enum Operation
    {
        Nop,
        Add,
        Subtract,
        Multiply,
        Divide,
    }
    
    public class OperatorWheelComponent : MonoBehaviour, IBeginDragHandler,IDragHandler, IEndDragHandler
    {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;
 
        public Operation currentOperation = Operation.Nop;
        [SerializeField] private TextMeshProUGUI OperationText;
        [SerializeField] GameObject Cylinder;
    
        Vector2 _dragStart, _dragEnd;
        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvas.worldCamera = Camera.main;
            
            UpdateOp();
        }
    

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStart = eventData.position;
            Debug.Log("Drag Started!");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragEnd = eventData.position;
            Debug.Log("Drag Stopped");
            CalculateSwipe();
        }

        public void CalculateSwipe()
        {
            Vector2 direction = (_dragEnd - _dragStart).normalized;
            if (direction.y < -0.4f)
            {
                Debug.Log("Swipe down");
                StartCoroutine(RotateDown());
                ShiftOp(true);
            }

            if (direction.y > 0.4f)
            {
                Debug.Log("Swipe Up");
                StartCoroutine(RotateUp());
                ShiftOp(false);
            }

            UpdateOp();
            //TODO Visual Update
        }

        private void UpdateOp()
        {
            OperationText.text = currentOperation switch {
                Operation.Nop => "Nop",
                Operation.Add => "+",
                Operation.Subtract => "-",
                Operation.Multiply => "\u00d7",
                Operation.Divide => "\u00f7",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void ShiftOp(bool direction)
        {
            if (direction)
            {
                currentOperation++;
                if ((int)currentOperation >= Enum.GetValues(typeof(Operation)).Length)
                {
                    currentOperation = 0;
                }
                return;
            }
            
            currentOperation--;
            if ((int)currentOperation < 0)
            {
                currentOperation = (Operation)(Enum.GetValues(typeof(Operation)).Length - 1);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Dragging!");
        }

        public IEnumerator RotateDown()
        {
            for (int i = 0; i < 9; i++)
            {
                Cylinder.transform.Rotate(Vector3.right, -10f, Space.World);
                yield return new WaitForSeconds(1f / 60f);
            }
        }
        public IEnumerator RotateUp()
        {
            for (int i = 0; i < 9; i++)
            {
                Cylinder.transform.Rotate(Vector3.right, 10f, Space.World);
                yield return new WaitForSeconds(1f / 60f);
            }
        }

        
    }
}
