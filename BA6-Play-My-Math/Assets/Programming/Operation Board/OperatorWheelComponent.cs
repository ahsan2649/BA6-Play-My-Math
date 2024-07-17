using System;
using System.Collections;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Programming.Operation_Board {
    public class OperatorWheelComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;

        public Operation currentOperation = Operation.Add;
        [SerializeField] private TextMeshProUGUI OperationText;
        [SerializeField] GameObject Cylinder;
        [SerializeField] private Animator animator; 
        
        public UnityEvent OnChangeOperation;

        Vector2 _dragStart, _dragEnd;

        [SerializeField] private float spinSpeed = 20f;
        private static readonly int SpinLeft = Animator.StringToHash("SpinLeft");
        private static readonly int SpinRight = Animator.StringToHash("SpinRight");

        //@Ahsan: previously Awake and Start in onEnable, but that runs simultaneously(/before) OperationBoardComponent.Awake(), but UpdateOp needs OperationBoardComponent.Instance
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
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
            if (direction.x < -0.4f)
            {
                Debug.Log("Swipe down");
                // StartCoroutine(RotateLeft()); //ZyKa!
                animator.SetTrigger(SpinLeft);
                ShiftOp(true);
            }

            if (direction.x > 0.4f)
            {
                Debug.Log("Swipe Up");
                // StartCoroutine(RotateRight()); ZyKa!
                animator.SetTrigger(SpinRight);
                ShiftOp(false);
            }

            UpdateOp();
        }

        private void UpdateOp()
        {
            OperationText.text = currentOperation switch
            {
                Operation.Nop => "Nop",
                Operation.Add => "+",
                Operation.Subtract => "-",
                Operation.Multiply => "\u00d7",
                Operation.Divide => "\u00f7",
                _ => throw new ArgumentOutOfRangeException()
            };

            OnChangeOperation.Invoke();
        }

        private void ShiftOp(bool direction)
        {
            if (direction)
            {
                currentOperation = currentOperation switch
                {
                    Operation.Add => Operation.Subtract,
                    Operation.Subtract => Operation.Multiply,
                    Operation.Multiply => Operation.Divide,
                    Operation.Divide => Operation.Add,
                    _ => throw new ArgumentOutOfRangeException()
                };
                return;
            }

            currentOperation = currentOperation switch
            {
                Operation.Add => Operation.Divide,
                Operation.Subtract => Operation.Add,
                Operation.Multiply => Operation.Subtract,
                Operation.Divide => Operation.Multiply,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Dragging!");
        }

        public IEnumerator RotateLeft()
        {
            var fromAngle = Cylinder.transform.rotation;
            var toAngle = Quaternion.Euler(Cylinder.transform.eulerAngles + new Vector3(0, 0, -90));

            while (Cylinder.transform.rotation != toAngle)
            {
                Cylinder.transform.rotation =
                    Quaternion.RotateTowards(Cylinder.transform.rotation, toAngle, spinSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator RotateRight()
        {
            var fromAngle = Cylinder.transform.rotation;
            var toAngle = Quaternion.Euler(Cylinder.transform.eulerAngles + new Vector3(0, 0, 90));

            while (Cylinder.transform.rotation != toAngle)
            {
                Cylinder.transform.rotation =
                    Quaternion.RotateTowards(Cylinder.transform.rotation, toAngle, spinSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}