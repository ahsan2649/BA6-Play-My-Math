using System;
using Programming.Card_Mechanism;
using Programming.ExtensionMethods;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Programming.Operation_Board {
    public class OperandSlotComponent : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
        [HideInInspector] public HandSlotComponent _originSlot;

        
        public NumberCardComponent CardInSlot
        {
            get => _cardInSlot;
            set
            {
                _cardInSlot = value;
                OperationBoardComponent.Instance._fractionVisualizer.VisualiseFraction(CardInSlot?.Value, visType);
            }
        }

        [Tooltip("left or right slot")]
        public FractionVisualizer.VisualisationType visType;

        [SerializeField] [HideInInspector] private NumberCardComponent _cardInSlot;

        void Start()
        {
            Debug.Log("OperandSlot working!");
        }

        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag;
            if (droppedCard == null)
            {
                return;
            }

            var droppedCardNumberComponent = droppedCard.GetComponent<NumberCardComponent>();
            if (droppedCardNumberComponent == null)
            {
                return;
            }

            if (droppedCardNumberComponent.Value.IsWhole())
            {
                return;
            }

            if (CardInSlot == null)
            {
                _originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
                CardInSlot = droppedCardNumberComponent;
                droppedCard.transform.SetParent(transform);

                StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().MoveToNewParent());
                StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().RotateToNewParent());
                
                return;
            }

            CardInSlot.transform.SetParent(_originSlot.transform);
            StartCoroutine(CardInSlot.GetComponent<BaseCardComponent>().MoveToNewParent());
            StartCoroutine(CardInSlot.GetComponent<BaseCardComponent>().RotateToNewParent());

            _originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            CardInSlot = droppedCardNumberComponent;
            droppedCard.transform.SetParent(transform);

            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().MoveToNewParent());
            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().RotateToNewParent());
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            var draggedCard = eventData.pointerDrag;
            if (draggedCard == null)
            {
                return;
            }

            var draggedCardNumber = draggedCard.GetComponent<NumberCardComponent>();
            if (draggedCardNumber != CardInSlot)
            {
                return;
            }
            
            _originSlot.SetCard(draggedCardNumber.GetComponent<BaseCardComponent>());
            _originSlot = null;
            CardInSlot = null;
        }
    }
}