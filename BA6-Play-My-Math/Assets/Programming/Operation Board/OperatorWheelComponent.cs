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
    public class OperatorWheelComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        Canvas _canvas;

        public List<Operation> availableOperations = new();
        public Operation currentOperation;
        [SerializeField] private TextMeshProUGUI OperationText;
        [SerializeField] GameObject Cylinder;
        [SerializeField] private Animator animator; 
        
        [SerializeField] private GameObject[] wheelObjects; 
        [SerializeField] private GameObject[] plateObjects; 
        
        public UnityEvent OnChangeOperation;

        Vector2 _dragStart, _dragEnd;

        [SerializeField] private float spinSpeed = 20f;
        private static readonly int SpinLeft = Animator.StringToHash("SpinLeft");
        private static readonly int SpinRight = Animator.StringToHash("SpinRight");

        //@Ahsan: previously Awake and Start in onEnable, but that runs simultaneously(/before) OperationBoardComponent.Awake(), but UpdateOp needs OperationBoardComponent.Instance
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            currentOperation = availableOperations[0];
        }

        private void Start()
        {
            _canvas.worldCamera = Camera.main;

            UpdateOp();
        }

        public void SetWheelActive(bool bActive)
        {
            foreach (GameObject gO in wheelObjects)
            {
                gO.SetActive(bActive);
            }

            enabled = true; 
        }

        public void SetPlateActive(bool bActive)
        {
            foreach (GameObject plateObject in plateObjects)
            {
                plateObject.SetActive(bActive);
            }
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
            var currentIndex = availableOperations.IndexOf(currentOperation);
            if (direction)
            {
                currentOperation = availableOperations[(currentIndex + 1) % availableOperations.Count];
                return;
            }

            currentOperation = availableOperations[Mathf.Abs((currentIndex - 1) % availableOperations.Count)];
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Dragging!");
        }

        public IEnumerator RotateLeft() //deprecated
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

        public IEnumerator RotateRight() //deprecated
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