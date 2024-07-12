using Programming.Fraction_Engine;
using Programming.Visualisers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace Programming.Card_Mechanism
{
    public class NumberCardComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        public Fraction oldValue;
        [SerializeField] Fraction value;

        public UnityEvent onValueChange;
        
        public bool IsFractionPreview = false; 
        public bool IsFraction = false;
        public bool CanCombineIntoFraction => IsFractionPreview || Value.IsWhole(); //needs to check for IsFraction as well, because Fractions are not over one while in preview mode, but are not counted as fractions  
        
        public Fraction Value
        {
            get => value;
            set
            {
                this.value = value;
                onValueChange.Invoke(); 
            }
        }



        private void Awake()
        {
            Value = Value; 
        }

        #region Pointer

        public void OnPointerEnter(PointerEventData eventData)
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
            
            if (!draggedCardNumber.CanCombineIntoFraction || !CanCombineIntoFraction)
            {
                return;
            }

            draggedCardNumber.IsFractionPreview = true;
            draggedCardNumber.oldValue = draggedCardNumber.Value;
            draggedCardNumber.Value = new Fraction(draggedCardNumber.Value.Numerator, Value.Numerator);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var draggedCard = eventData.pointerDrag;
            if (draggedCard == null)
            {
                return;
            }

            var draggedCardNumber = eventData.pointerDrag.GetComponent<NumberCardComponent>();
            if (draggedCardNumber == null)
            {
                return;
            }
            
            draggedCardNumber.IsFractionPreview = false; 
            draggedCardNumber.Value = draggedCardNumber.oldValue;
        }

        #endregion

        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag;
            if (droppedCard == null)
            {
                return;
            }
            
            var droppedCardNumber = droppedCard.GetComponent<NumberCardComponent>();
            
            if (droppedCardNumber.IsFraction && IsFraction)
            {
                GetComponent<CardMovementComponent>().currentSlot.SwapCards(droppedCard.GetComponent<CardMovementComponent>().currentSlot, droppedCard.GetComponent<CardMovementComponent>());
                return; 
            }
            
            if (!droppedCardNumber.CanCombineIntoFraction || !CanCombineIntoFraction)
            {
                return;
            }
            
            // Step 1: Remove DroppedCard from its slot in the player hand
            var droppedCardSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            PlayerHandComponent.Instance.HandPop(ref droppedCardSlot);

            // Step 2: Update Dropped Card with the fraction made by the Zählers
            droppedCardNumber.IsFraction = true;
            droppedCardNumber.oldValue = droppedCardNumber.Value = new Fraction(droppedCardNumber.Value.Numerator, value.Numerator);
            
            // Step 3: Set dropped card to the slot this card is in
            var thisCardSlot = GetComponentInParent<HandSlotComponent>();
            thisCardSlot.SetCard(droppedCard.GetComponent<CardMovementComponent>());
            
            // Draw another card and destroy this one
            PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
            Destroy(gameObject);
        }
    }
}