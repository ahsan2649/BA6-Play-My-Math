using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Card_Mechanism {
    public class BinComponent : MonoBehaviour, IDropHandler {
        private List<BaseCardComponent> _disCards = new List<BaseCardComponent>();

        public void PutCardInBin(BaseCardComponent baseCard)
        {
            _disCards.Add(baseCard);
            
            baseCard.transform.SetParent(transform);
            baseCard.transform.position = transform.position;
            baseCard.transform.rotation = Quaternion.identity;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var playerHand = GameObject.Find("Player Hand").GetComponent<PlayerHandComponent>();
            var droppedCardSlot = eventData.pointerDrag.GetComponentInParent<CardSlotComponent>();
            var droppedCard = playerHand.HandPop(ref droppedCardSlot); 
            PutCardInBin(droppedCard);
            StartCoroutine(droppedCard.DiscardAnimation());
            
            playerHand.HandPush(GameObject.Find("Deck").GetComponent<DeckComponent>().DeckPop());;
        }
    }
}
