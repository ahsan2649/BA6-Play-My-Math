using UnityEngine;

namespace Programming.Card_Mechanism {
    public class CardSlotComponent : MonoBehaviour {
        private BaseCardComponent _baseCardInSlot;
        

        public void SetCard(BaseCardComponent baseCard)
        {
            _baseCardInSlot = baseCard;
            baseCard.transform.SetParent(this.transform);
            StartCoroutine(baseCard.MoveToNewParent());
            StartCoroutine(baseCard.RotateToNewParent());
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

        public bool HasCard() { return _baseCardInSlot != null; }
    }
}
