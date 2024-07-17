using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Card_Mechanism {
    public class DiscardPileComponent : MonoBehaviour, IDropHandler {
        private List<CardMovementComponent> _disCards = new List<CardMovementComponent>();

        public UnityEvent onDiscardPileChanged;  
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
            onDiscardPileChanged.Invoke(); 
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

        public void ClearPile()
        {
            for (var index = _disCards.Count - 1; index >= 0; index--)
            {
                var card = _disCards[index];
                _disCards.Remove(card);
                Destroy(card.gameObject);
            }
        }
    }
}
