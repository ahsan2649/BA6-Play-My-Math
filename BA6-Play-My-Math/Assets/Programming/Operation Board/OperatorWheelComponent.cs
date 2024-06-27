using System;
using System.Collections;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board
{
    [Serializable]
    public class OperationTextPair
    {
        public Operation operation;
        public GameObject value;
    }
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
        [SerializeField] List<OperationTextPair> _operationTextPairs;
        public Operation currentOperation = Operation.Nop;

        [SerializeField] GameObject Cylinder;
    
        Vector2 _dragStart, _dragEnd;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvas.worldCamera = Camera.main;
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
            this.GetComponentInSiblings<FractionVisualizer>().RemoveVisuals(FractionVisualizer.VisualisationInputType.Operator);
            this.GetComponentInSiblings<FractionVisualizer>().AddVisuals(FractionVisualizer.VisualisationInputType.Operator);
        }

        private void UpdateOp()
        {
            foreach (var pair in _operationTextPairs)
            {
                pair.value?.SetActive(false);
                if (currentOperation == pair.operation)
                {
                    pair.value?.SetActive(true);
                }
            }
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
            for (int i = 0; i < 30; i++)
            {
                Cylinder.transform.Rotate(Vector3.right, -3f, Space.World);
                yield return new WaitForSeconds(1f / 30f);
            }
        }
        public IEnumerator RotateUp()
        {
            for (int i = 0; i < 30; i++)
            {
                Cylinder.transform.Rotate(Vector3.right, 3f, Space.World);
                yield return new WaitForSeconds(1f / 30f);
            }
        }

        
    }
}
