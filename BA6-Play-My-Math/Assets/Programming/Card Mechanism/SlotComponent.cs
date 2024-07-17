using UnityEngine;
using UnityEngine.Events;

namespace Programming.Card_Mechanism
{
    public abstract class SlotComponent : MonoBehaviour
    {
        protected CardMovementComponent _cardMovementInSlot;
        
        public UnityEvent onCardChanged; 
        public void OnCardChanged()
        {
            onCardChanged.Invoke(); 
        }
        
        public void SwapCards(SlotComponent otherSlot, CardMovementComponent otherCard)
        {
            CardMovementComponent thisCard = null; 
            if (_cardMovementInSlot != null)
            {
                thisCard = UnsetCard();
            }
            otherSlot.UnsetCard();
            SetCard(otherCard);
            if (thisCard is not null)
            {
                otherSlot.SetCard(thisCard);
                thisCard.TransformToNewParentCoroutines();
            }
        }
        
        public virtual void SetCard(CardMovementComponent cardMovement)
        {
            _cardMovementInSlot = cardMovement;
            cardMovement.transform.SetParent(transform);
            cardMovement.currentSlot = this;
            cardMovement.enabled = true;
            
            onCardChanged.Invoke(); 
        }
        
        public CardMovementComponent UnsetCard()
        {
            if (_cardMovementInSlot is null)
            {
                return null; 
            }
            
            var returningCard = _cardMovementInSlot;
            returningCard.currentSlot = null;
            _cardMovementInSlot = null;

            onCardChanged.Invoke(); 
            return returningCard;
        }
        
        public CardMovementComponent GetCard()
        {
            return _cardMovementInSlot;
        }

        public bool HasCard() => _cardMovementInSlot != null;
    }
}
