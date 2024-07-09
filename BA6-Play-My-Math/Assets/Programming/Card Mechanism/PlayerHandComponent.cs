using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Programming.Card_Mechanism
{
    public class PlayerHandComponent : MonoBehaviour
    {
        public static PlayerHandComponent Instance { get; private set; }
        private HandSlotComponent[] cardSlots;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            cardSlots = GetComponentsInChildren<HandSlotComponent>();
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                HandPush(DeckComponent.Instance.DeckPop());
            }
        }


        public void HandPush(BaseCardComponent baseCard)
        {
            foreach (HandSlotComponent slot in cardSlots)
            {
                if (slot.HasCard())
                {
                    continue;
                }

                if (baseCard != null)
                {
                    slot.SetCard(baseCard);
                    StartCoroutine(baseCard.MoveToNewParent());
                    StartCoroutine(baseCard.RotateToNewParent());
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