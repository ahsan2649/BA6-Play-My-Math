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

        void Start()
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                HandPush(DeckComponent.Instance.DeckPop());
            }
        }


        public void HandPush(CardMovementComponent cardMovement, bool shouldMove = true)
        {
            foreach (HandSlotComponent slot in cardSlots)
            {
                if (slot.HasCard())
                {
                    continue;
                }

                if (cardMovement != null)
                {
                    slot.SetCard(cardMovement);
                    if (shouldMove)
                    {
                        cardMovement.TransformToNewParentCoroutines();
                    }
                }

                break;
            }
        }

        public CardMovementComponent HandPop(ref HandSlotComponent slot)
        {
            var returningCard = slot.UnsetCard();
            return returningCard;
        }
    }
}