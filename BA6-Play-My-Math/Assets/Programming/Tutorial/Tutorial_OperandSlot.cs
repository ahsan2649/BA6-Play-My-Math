using System.Collections.Generic;
using System.Linq;
using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Tutorial
{
    public class Tutorial_OperandSlot : TutorialElement
    {
        [SerializeField] private List<SlotComponent> slotComponents;
        [SerializeField] private TutorialElement previousTutorial; 
        
        protected override List<UnityEvent> GetActivationEvents()
        {
            return new List<UnityEvent>(){previousTutorial.onTutorialStepFinished}; 
        }

        protected override List<UnityEvent> GetFinishEvents()
        {
            return slotComponents.Select(slot => slot.onCardChanged).ToList(); 
        }

        protected override bool CheckFinishCondition()
        {
            foreach (SlotComponent slot in slotComponents)
            {
                if (slot.GetCard() is null)
                {
                    return false; 
                }
            }
            return true; 
        }
    }
}
