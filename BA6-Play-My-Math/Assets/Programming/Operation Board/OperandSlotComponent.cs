using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board {
    public class OperandSlotComponent : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
        public HandSlotComponent _originSlot;

        public NumberCardComponent _cardInSlot;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("OperandSlot working!");
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag;
            if (droppedCard == null)
            {
                return;
            }

            var droppedCardNumberComponent = droppedCard.GetComponent<NumberCardComponent>();
            if (droppedCardNumberComponent == null)
            {
                return;
            }

            if (droppedCardNumberComponent.Value.IsWhole())
            {
                return;
            }

            if (_cardInSlot == null)
            {
                _originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
                _cardInSlot = droppedCardNumberComponent;
                droppedCard.transform.SetParent(transform);

                StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().MoveToNewParent());
                StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().RotateToNewParent());
                return;
            }

            _cardInSlot.transform.SetParent(_originSlot.transform);
            StartCoroutine(_cardInSlot.GetComponent<BaseCardComponent>().MoveToNewParent());
            StartCoroutine(_cardInSlot.GetComponent<BaseCardComponent>().RotateToNewParent());

            _originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            _cardInSlot = droppedCardNumberComponent;
            droppedCard.transform.SetParent(transform);

            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().MoveToNewParent());
            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().RotateToNewParent());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            var draggedCard = eventData.pointerDrag;
            if (draggedCard == null)
            {
                return;
            }

            var draggedCardNumber = draggedCard.GetComponent<NumberCardComponent>();
            if (draggedCardNumber != _cardInSlot)
            {
                return;
            }
            
            _originSlot.SetCard(draggedCardNumber.GetComponent<BaseCardComponent>());
            _originSlot = null;
            _cardInSlot = null;
        }
    }
}