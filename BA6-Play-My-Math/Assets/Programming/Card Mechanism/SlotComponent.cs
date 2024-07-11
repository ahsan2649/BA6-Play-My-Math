using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Programming.Card_Mechanism
{
    public abstract class SlotComponent : MonoBehaviour
    {
        private CardMovementComponent _cardMovementInSlot;
        
        public UnityEvent onCardChanged; 
        
        public void SetCard(CardMovementComponent card)
        {
            if (_cardMovementInSlot is not null)
            {
                UnsetCard(); 
            }
            
            _cardMovementInSlot = card;
            
            if (card is not null)
            {
                card.transform.SetParent(transform);
                card.enabled = true;
                card.onCardChange.AddListener(OnCardChanged);
            }
            
            onCardChanged.Invoke(); 
        }
        
        public CardMovementComponent UnsetCard()
        {
            if (_cardMovementInSlot is null)
            {
                return null; 
            }
            
            _cardMovementInSlot.onCardChange.RemoveListener(OnCardChanged);
            
            var returningCard = _cardMovementInSlot;
            _cardMovementInSlot = null;

            onCardChanged.Invoke(); //ZyKa!
            return returningCard;
        }

        public void OnCardChanged()
        {
            onCardChanged.Invoke();
        }
        
        public CardMovementComponent GetCard()
        {
            return _cardMovementInSlot;
        }

        public bool HasCard() => _cardMovementInSlot != null;
    }
}
