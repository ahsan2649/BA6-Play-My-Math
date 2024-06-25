using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Card_Mechanism {
    public class BinComponent : MonoBehaviour, IDropHandler, IPointerEnterHandler {
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

            var droppedCard = eventData.pointerDrag.GetComponent<BaseCardComponent>();
            var droppedCardSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            
            if (droppedCardSlot != null)
            {
                playerHand.HandPop(ref droppedCardSlot);
            }
            
            
            PutCardInBin(droppedCard);
            StartCoroutine(droppedCard.DiscardAnimation());
            
            playerHand.HandPush(GameObject.Find("Deck").GetComponent<DeckComponent>().DeckPop());;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer enter bin!");
        }
    }
}
