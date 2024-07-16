using System.Collections.Generic;
using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Tutorial
{
    public class Tutorial_Throwaway : TutorialElement
    {
        [SerializeField] private DeckComponent _deckComponent;
        [SerializeField] private TutorialElement previousTutorial; 
        
        protected override List<UnityEvent> GetActivationEvents()
        {
            return new List<UnityEvent>(){previousTutorial.onTutorialStepFinished}; 
        }

        protected override List<UnityEvent> GetFinishEvents()
        {
            return new List<UnityEvent>(){ _deckComponent.onDeckChanged}; 
        }

        protected override bool CheckFinishCondition()
        {
            return true; 
        }
    }
}
