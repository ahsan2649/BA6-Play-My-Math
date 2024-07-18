using System.Collections.Generic;
using System.Linq;
using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Tutorial
{
    public class Tutorial_DragCard : TutorialElement
    {
        [SerializeField] private List<SlotComponent> slotComponents;
        
        protected override List<UnityEvent> GetFinishEvents()
        {
            return slotComponents.Select(slot => slot.onCardChanged).ToList(); 
        }

        protected override bool CheckFinishCondition()
        {
            foreach (SlotComponent slot in slotComponents)
            {
                NumberCardComponent numberCard = slot.GetCard()?.GetComponent<NumberCardComponent>(); 
                if (numberCard is not null && numberCard.IsFraction)
                {
                    return true; 
                }
            }
            return false; 
        }
    }
}
