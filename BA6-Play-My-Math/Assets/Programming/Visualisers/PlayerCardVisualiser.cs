using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Visualisers
{
    public class NumberCardVisualiser : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public NumberCardComponent numberCardComponent;
        public ExpandSimplifyCard expandSimplifyCard; 
        [SerializeField] private FractionTextVisualiser fractionTextVisualiser; 

        public void OnNumberCardChanged()
        {
            fractionTextVisualiser.SetFraction(numberCardComponent.Value, numberCardComponent.IsFraction);
            gameObject.name = "Card(" + numberCardComponent.Value + ")"; 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            NumberCardComponent draggedCard = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<NumberCardComponent>() : null; 
            
            if (draggedCard != null &&
                !numberCardComponent.IsFraction)
            {
                fractionTextVisualiser.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            fractionTextVisualiser.gameObject.SetActive(true); 
        }
    }
}
