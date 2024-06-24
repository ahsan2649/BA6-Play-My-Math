using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Programming.Card_Mechanism {
    public class NumberCardComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler {
        [SerializeField] Fraction value;
        public Fraction oldValue;

        public Fraction Value
        {
            get => value;
            set
            {
                this.value = value;
                UpdateValue();
            }
        }

        private void OnEnable()
        {
            UpdateValue();
        }

        public void UpdateValue()
        {
            if (Value.Denominator == 1)
            {
                transform.Find("Number").gameObject.SetActive(true);
                transform.Find("Fraction").gameObject.SetActive(false);
                transform.Find("Number").GetComponent<TextMeshProUGUI>().text = Value.Numerator.ToString();
                return;
            }

            transform.Find("Number").gameObject.SetActive(false);
            transform.Find("Fraction").gameObject.SetActive(true);
            transform.Find("Fraction").Find("Zähler").GetComponent<TextMeshProUGUI>().text = Value.Numerator.ToString();
            transform.Find("Fraction").Find("Nenner").GetComponent<TextMeshProUGUI>().text =
                Value.Denominator.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null || !value.IsWhole() ||
                !eventData.pointerDrag.GetComponent<NumberCardComponent>().value.IsWhole() || value.IsOne())
            {
                return;
            }

            var draggedCard = eventData.pointerDrag.GetComponent<NumberCardComponent>();
            draggedCard.oldValue = draggedCard.Value;
            draggedCard.Value = new Fraction(draggedCard.Value.Numerator, Value.Numerator);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null || !value.IsWhole() ||
                !eventData.pointerDrag.GetComponent<NumberCardComponent>().oldValue.IsWhole() || value.IsOne()) 
            {
                Debug.Log("Dragging nothing!");
                return;
            }

            var draggedCard = eventData.pointerDrag.GetComponent<NumberCardComponent>();
            draggedCard.Value = draggedCard.oldValue;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null || !value.IsWhole() ||
                !eventData.pointerDrag.GetComponent<NumberCardComponent>().oldValue.IsWhole() || value.IsOne())
            {
                Debug.Log("Dragging nothing!");
                return;
            }

            var playerHand = GameObject.Find("Player Hand").GetComponent<PlayerHandComponent>();
            var droppedCardSlot = eventData.pointerDrag.GetComponentInParent<HandSlotComponent>();
            var newCard = playerHand.HandPop(ref droppedCardSlot);

            var newCardNumber = newCard.GetComponent<NumberCardComponent>();
            var thisCardSlot = GetComponentInParent<HandSlotComponent>();
            
            newCardNumber.oldValue = newCardNumber.Value = new Fraction(newCardNumber.Value.Numerator, value.Numerator);
            thisCardSlot.SetCard(newCard);
            playerHand.HandPush(GameObject.Find("Deck").GetComponent<DeckComponent>().DeckPop());
            
            Destroy(gameObject);

        }
    }
}