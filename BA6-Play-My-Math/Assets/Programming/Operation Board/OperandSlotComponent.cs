using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board {
    public class OperandSlotComponent : MonoBehaviour, IDropHandler, IPointerEnterHandler {
        private HandSlotComponent _originSlot;

        private NumberCardComponent _cardInSlot;

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
                Debug.Log("Dropped nothing!");
                return;
            }

            var droppedCardNumberComponent = droppedCard.GetComponent<NumberCardComponent>();
            if (droppedCardNumberComponent == null)
            {
                Debug.Log("Not dropping a number type card");
                return;
            }

            if (droppedCardNumberComponent.Value.IsWhole())
            {
                Debug.Log("Not dropping a fraction card");
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
            Debug.Log("Pointer Enter!");
        }
    }
}