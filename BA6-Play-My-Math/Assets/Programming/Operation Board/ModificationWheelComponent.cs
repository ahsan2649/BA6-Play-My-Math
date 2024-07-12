using System;
using System.Collections;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Programming.Operation_Board {
    public class ModificationWheelComponent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler {
        [SerializeField] ModifyType _modifyType;
        [SerializeField] ModifyValue _currentModifyValue;

        [SerializeField] GameObject Cylinder;
        [SerializeField] TextMeshProUGUI modifyValueText;
        [SerializeField] TextMeshProUGUI modifyTypeText;

        
        Vector2 _dragStart, _dragEnd;
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _canvas.worldCamera = Camera.main;

            modifyTypeText.text = _modifyType switch
            {
                ModifyType.Simplify => "\u00f7",
                ModifyType.Expand => "\u00d7",
                ModifyType.None => "None", 
                _ => throw new ArgumentOutOfRangeException()
            };
            
            UpdateOp();
        }

        private void UpdateOp()
        {
            modifyValueText.text = _currentModifyValue switch
            {
                ModifyValue.Two => "2",
                ModifyValue.Three => "3",
                ModifyValue.Five => "5",
                ModifyValue.Seven => "7",
                _ => throw new ArgumentOutOfRangeException()
            };
            
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

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void CalculateSwipe()
        {
            Vector2 direction = (_dragEnd - _dragStart).normalized;
            if (direction.x < -0.4f)
            {
                Debug.Log("Swipe left");
                StartCoroutine(RotateLeft());
                ShiftOp(false);
            }

            if (direction.x > 0.4f)
            {
                Debug.Log("Swipe Right");
                StartCoroutine(RotateRight());
                ShiftOp(true);
            }

            UpdateOp();
            //TODO Visual Update
        }

        private void ShiftOp(bool direction)
        {
            if (direction)
            {
                _currentModifyValue = _currentModifyValue switch
                {
                    ModifyValue.Two => ModifyValue.Seven,
                    ModifyValue.Three => ModifyValue.Two,
                    ModifyValue.Five => ModifyValue.Three,
                    ModifyValue.Seven => ModifyValue.Five,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return;
            }


            _currentModifyValue = _currentModifyValue switch
            {
                ModifyValue.Two => ModifyValue.Three,
                ModifyValue.Three => ModifyValue.Five,
                ModifyValue.Five => ModifyValue.Seven,
                ModifyValue.Seven => ModifyValue.Two,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public IEnumerator RotateRight()
        {
            for (int i = 0; i < 9; i++)
            {
                Cylinder.transform.Rotate(Vector3.up, -10f, Space.World);
                yield return new WaitForSeconds(1f / 60f);
            }
        }

        public IEnumerator RotateLeft()
        {
            for (int i = 0; i < 9; i++)
            {
                Cylinder.transform.Rotate(Vector3.up, 10f, Space.World);
                yield return new WaitForSeconds(1f / 60f);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            var draggedCard = eventData.pointerDrag;
            if (draggedCard == null)
            {
                return;
            }

            var draggedCardNumber = draggedCard.GetComponent<NumberCardComponent>();
            if (!draggedCardNumber.IsFraction && !draggedCardNumber.Value.IsOne())
            {
                return;
            }

            draggedCardNumber.oldValue = draggedCardNumber.Value = _modifyType switch
            {
                ModifyType.Simplify => draggedCardNumber.Value.SimplifyBy((int)_currentModifyValue),
                ModifyType.Expand => draggedCardNumber.Value.ExpandBy((int)_currentModifyValue),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}