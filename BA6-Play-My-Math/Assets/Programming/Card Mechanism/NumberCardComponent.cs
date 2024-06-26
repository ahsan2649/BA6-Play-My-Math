using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Programming.Card_Mechanism
{
    public class NumberCardComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        public Fraction oldValue;
        [SerializeField] Fraction value;

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

        #region Pointer

        public void OnPointerEnter(PointerEventData eventData)
        {
            var draggedCard = eventData.pointerDrag;
            if (draggedCard == null)
            {
                return;
            }

            var draggedCardNumber = draggedCard.GetComponent<NumberCardComponent>();
            if (!draggedCardNumber.Value.IsWhole() || !Value.IsWhole() || Value.IsOne())
            {
                return;
            }

            transform.Find("Number").gameObject.SetActive(false);

            draggedCardNumber.oldValue = draggedCardNumber.Value;
            draggedCardNumber.Value = new Fraction(draggedCardNumber.Value.Numerator, Value.Numerator);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var draggedCard = eventData.pointerDrag;
            if (draggedCard == null)
            {
                return;
            }

            var draggedCardNumber = eventData.pointerDrag.GetComponent<NumberCardComponent>();
            if (!draggedCardNumber.oldValue.IsWhole() || !value.IsWhole() || value.IsOne())
            {
                return;
            }
            
            transform.Find("Number").gameObject.SetActive(true);
            draggedCardNumber.Value = draggedCardNumber.oldValue;
        }

        #endregion

        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag;
            if (droppedCard == null)
            {
                return;
            }

            var droppedCardNumber = droppedCard.GetComponent<NumberCardComponent>();
            if (!droppedCardNumber.oldValue.IsWhole() || !value.IsWhole() || value.IsOne())
            {
                return;
            }

            // Step 1: Remove DroppedCard from its slot in the player hand
            var droppedCardSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            PlayerHandComponent.Instance.HandPop(ref droppedCardSlot);

            // Step 2: Update Dropped Card with the fraction made by the Zählers
            droppedCardNumber.oldValue = droppedCardNumber.Value = new Fraction(droppedCardNumber.Value.Numerator, value.Numerator);

            // Step 3: Set dropped card to the slot this card is in
            var thisCardSlot = GetComponentInParent<HandSlotComponent>();
            thisCardSlot.SetCard(droppedCard.GetComponent<BaseCardComponent>());
            
            // Draw another card and destroy this one
            PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
            Destroy(gameObject);
        }
    }
}