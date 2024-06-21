using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Programming.Card_Mechanism {
    public class PlayerHandComponent : MonoBehaviour {
        private CardSlotComponent[] _cardSlots;

        private void Awake()
        {
            _cardSlots = GetComponentsInChildren<CardSlotComponent>();
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
            foreach (CardSlotComponent slot in _cardSlots)
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

        public BaseCardComponent HandPop(ref CardSlotComponent slot)
        {
            var returningCard = slot.UnsetCard();
            return returningCard;
        }
    }
}
