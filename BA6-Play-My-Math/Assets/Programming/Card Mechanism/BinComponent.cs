using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Card_Mechanism {
    public class BinComponent : MonoBehaviour, IDropHandler, IPointerEnterHandler {
        private List<BaseCardComponent> _disCards = new List<BaseCardComponent>();
        public static BinComponent Instance { get; private set; }
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

        public void PutCardInBin(BaseCardComponent baseCard)
        {
            _disCards.Add(baseCard);
            
            baseCard.transform.SetParent(transform);
            baseCard.transform.position = transform.position;
            baseCard.transform.rotation = Quaternion.identity;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag.GetComponent<BaseCardComponent>();
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
