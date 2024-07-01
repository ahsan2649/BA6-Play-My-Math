using UnityEngine;

namespace Programming.Card_Mechanism
{
    public class HandSlotComponent : MonoBehaviour
    {
        private BaseCardComponent _baseCardInSlot;

        public void SetCard(BaseCardComponent baseCard)
        {
            _baseCardInSlot = baseCard;
            baseCard.transform.SetParent(transform, true);
            baseCard.enabled = true;
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