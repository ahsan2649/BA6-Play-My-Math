using System.Collections.Generic;
using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Tutorial
{
    public class Tutorial_Throwaway : TutorialElement
    {
        [SerializeField] private DiscardPileComponent discardPileComponent;
        [SerializeField] private TutorialElement previousTutorial; 
        
        protected override List<UnityEvent> GetFinishEvents()
        {
            return new List<UnityEvent>(){ discardPileComponent.onDiscardPileChanged}; 
        }

        protected override bool CheckFinishCondition()
        {
            return true; 
        }
    }
}
