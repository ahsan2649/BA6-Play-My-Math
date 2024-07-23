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
            if (_cardMovementInSlot is not null)
            {
                NumberCardComponent oldNumberCard = _cardMovementInSlot.GetComponent<NumberCardComponent>();
                if (oldNumberCard is not null)
                {
                    oldNumberCard.onValueChange.RemoveListener(CallOnCardChanged);
                }
            }
            base.SetCard(cardMovement);
            if (_cardMovementInSlot is not null)
            {
                NumberCardComponent newNumberCard = _cardMovementInSlot.GetComponent<NumberCardComponent>();
                if (newNumberCard is not null)
                {
                    newNumberCard.onValueChange.AddListener(CallOnCardChanged);
                }
            }
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

        public void SetActiveAndEnabled(bool bActive)
        {
            transform.parent.gameObject.SetActive(bActive);
            enabled = bActive; 
        }
    }
}