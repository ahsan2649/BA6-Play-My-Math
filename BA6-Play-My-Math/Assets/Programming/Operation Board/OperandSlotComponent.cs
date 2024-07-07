using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board
{
    public class OperandSlotComponent : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;
        
        [HideInInspector] public HandSlotComponent _originSlot;


        public NumberCardComponent CardInSlot
        {
            get => _cardInSlot;
            set
            {
                _cardInSlot = value;
                OperationBoardComponent.Instance.fractionVisualiser.SetFractionVisualisation(CardInSlot?.Value, visType);
                if (visType == FractionVisualiser.VisualisationType.Left)
                {
                    FightButtonComponent.Instance.EnableFighting(value != null ? value.Value : new Fraction(0, 1));
                }
            }
        }

        [Tooltip("left or right slot")] public FractionVisualiser.VisualisationType visType;

        [SerializeField] [HideInInspector] private NumberCardComponent _cardInSlot;

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvas.worldCamera = Camera.main;
        }
        
        void Start()
        {
            Debug.Log("OperandSlot working!");
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

            if (CardInSlot == null)
            {
                _originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
                CardInSlot = droppedCardNumberComponent;
                droppedCard.transform.SetParent(transform);

                StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().MoveToNewParent());
                StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().RotateToNewParent());

                return;
            }

            CardInSlot.transform.SetParent(_originSlot.transform);
            StartCoroutine(CardInSlot.GetComponent<BaseCardComponent>().MoveToNewParent());
            StartCoroutine(CardInSlot.GetComponent<BaseCardComponent>().RotateToNewParent());

            _originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            CardInSlot = droppedCardNumberComponent;
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
            if (draggedCardNumber != CardInSlot)
            {
                return;
            }

            _originSlot.SetCard(draggedCardNumber.GetComponent<BaseCardComponent>());
            _originSlot = null;
            CardInSlot = null;
        }
    }
}