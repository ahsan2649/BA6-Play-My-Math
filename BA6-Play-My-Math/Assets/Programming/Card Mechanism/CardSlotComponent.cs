using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Programming.Card_Mechanism
{
    public abstract class CardSlotComponent : MonoBehaviour
    {
        private BaseCardComponent _baseCardInSlot;
        public UnityEvent onCardChanged; 
        
        public void SetCard(BaseCardComponent baseCard)
        {
            _baseCardInSlot = baseCard;
            baseCard.transform.SetParent(transform);
            baseCard.enabled = true;
            
            baseCard.onCardChange.AddListener(OnCardChanged);
            onCardChanged.Invoke(); 
        }

        public void OnCardChanged()
        {
            onCardChanged.Invoke();
        }

        public BaseCardComponent UnsetCard()
        {
            var returningCard = _baseCardInSlot;
            _baseCardInSlot = null;
            return returningCard;
        }

        public BaseCardComponent GetCard()
        {
            return _baseCardInSlot;
        }

        public bool HasCard() => _baseCardInSlot != null;
    }
}
