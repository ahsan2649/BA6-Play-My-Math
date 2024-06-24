using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Programming.Card_Mechanism {
    public class PlayerHandComponent : MonoBehaviour {
        private HandSlotComponent[] _cardSlots;

        private void Awake()
        {
            _cardSlots = GetComponentsInChildren<HandSlotComponent>();
        }

        // Start is called before the first frame update
        void Start()
        {
            DeckComponent deck = GameObject.Find("Deck").GetComponent<DeckComponent>();
            for (int i = 0; i < _cardSlots.Length; i++)
            {
                HandPush(deck.DeckPop());
            }
        }


        public void HandPush(BaseCardComponent baseCard)
        {
            foreach (HandSlotComponent slot in _cardSlots)
            {
                if (slot.HasCard())
                {
                    continue;
                }

                if (baseCard != null)
                {
                    slot.SetCard(baseCard);
                }
                break;
            }
        }

        public BaseCardComponent HandPop(ref HandSlotComponent slot)
        {
            var returningCard = slot.UnsetCard();
            return returningCard;
        }
    }
}
