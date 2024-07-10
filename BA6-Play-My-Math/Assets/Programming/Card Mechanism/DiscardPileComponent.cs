using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Card_Mechanism {
    public class DiscardPileComponent : MonoBehaviour, IDropHandler, IPointerEnterHandler {
        private List<CardMovementComponent> _disCards = new List<CardMovementComponent>();
        public static DiscardPileComponent Instance { get; private set; }
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
        }

        public void PutCardInBin(CardMovementComponent cardMovement)
        {
            _disCards.Add(cardMovement);
            
            cardMovement.transform.SetParent(transform);
            cardMovement.transform.position = transform.position;
            cardMovement.transform.rotation = Quaternion.identity;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag.GetComponent<CardMovementComponent>();
            var droppedCardSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            
            if (droppedCardSlot != null)
            {
                PlayerHandComponent.Instance.HandPop(ref droppedCardSlot);
            }
            
            
            PutCardInBin(droppedCard);
            StartCoroutine(droppedCard.DiscardAnimation());
            
            PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer enter bin!");
        }
    }
}
