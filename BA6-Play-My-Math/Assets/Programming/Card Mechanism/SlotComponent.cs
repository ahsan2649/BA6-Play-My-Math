using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Programming.Card_Mechanism
{
    public abstract class SlotComponent : MonoBehaviour
    {
        private CardMovementComponent _cardMovementInSlot;
        
        public UnityEvent onCardChanged; 
        
        public void SetCard(CardMovementComponent cardMovement)
        {
            _cardMovementInSlot = cardMovement;
            cardMovement.transform.SetParent(transform);
            cardMovement.enabled = true;
            
            cardMovement.onCardChange.AddListener(OnCardChanged);
            onCardChanged.Invoke(); 
        }

        public void OnCardChanged()
        {
            onCardChanged.Invoke();
        }

        public CardMovementComponent UnsetCard()
        {
            _cardMovementInSlot.onCardChange.RemoveListener(OnCardChanged);
            
            var returningCard = _cardMovementInSlot;
            _cardMovementInSlot = null;

            onCardChanged.Invoke(); //ZyKa!
            return returningCard;
        }

        public CardMovementComponent GetCard()
        {
            return _cardMovementInSlot;
        }

        public bool HasCard() => _cardMovementInSlot != null;
    }
}
