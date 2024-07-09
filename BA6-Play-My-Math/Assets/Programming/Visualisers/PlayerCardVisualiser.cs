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
            fractionTextVisualiser.SetFraction(numberCardComponent.Value);
            gameObject.name = "Card(" + numberCardComponent.Value + ")"; 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            NumberCardComponent draggedCard = eventData.pointerDrag != null ? eventData.pointerDrag.GetComponent<NumberCardComponent>() : null; 
            Debug.Log(gameObject.name + "-draggedCard: " + draggedCard);
            
            if (draggedCard != null &&
                numberCardComponent.Value.IsWhole())
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
