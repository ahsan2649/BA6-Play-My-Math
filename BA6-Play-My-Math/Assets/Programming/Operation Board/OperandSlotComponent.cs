using System.Transactions;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Programming.Operation_Board
{
    public class OperandSlotComponent : SlotComponent, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        Canvas _canvas;
        
        public override void SetCard(CardMovementComponent cardMovement)
        {
            if (_cardMovementInSlot is not null) {_cardMovementInSlot.GetComponent<NumberCardComponent>()?.onValueChange.RemoveListener(onCardChanged.Invoke);}
            base.SetCard(cardMovement);
            if (_cardMovementInSlot is not null) { _cardMovementInSlot.GetComponent<NumberCardComponent>()?.onValueChange.AddListener(onCardChanged.Invoke); }
            // onCardChanged.Invoke(); already called in base
        }

        [FormerlySerializedAs("cardSlotType")] [Tooltip("left or right slot")] public OperandType operandType;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            
            _canvas.worldCamera = Camera.main;
        }
        
        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag;
            if (droppedCard == null)
            {
                return;
            }

            NumberCardComponent droppedCardNumberComponent = droppedCard.GetComponent<NumberCardComponent>();
            if (droppedCardNumberComponent == null)
            {
                return;
            }

            if (!droppedCardNumberComponent.IsFraction)
            {
                return;
            }

            CardMovementComponent droppedCardMovementComponent = droppedCard.GetComponent<CardMovementComponent>();

            SwapCards(droppedCardMovementComponent.currentSlot, droppedCardMovementComponent);
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
            if (draggedCardNumber == null)
            {
                return;
            }
            
            var cardMovementComponent = draggedCardNumber.GetComponent<CardMovementComponent>();
            if (cardMovementComponent != GetCard())
            {
                return;
            }

            UnsetCard();
            PlayerHandComponent.Instance.HandPush(cardMovementComponent, false);
        }
    }
}