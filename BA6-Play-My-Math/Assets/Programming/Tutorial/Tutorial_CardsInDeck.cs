using System.Collections.Generic;
using Programming.Visualisers;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Tutorial
{
    public class Tutorial_CardsInDeck: TutorialElement
    {
        [SerializeField] private DeckComponentVisualiser deckComponentVisualiser;
        [SerializeField] private TutorialElement previousTutorial; 
        
        protected override List<UnityEvent> GetActivationEvents()
        {
            return new List<UnityEvent>(){previousTutorial.onTutorialStepFinished}; 
        }

        protected override List<UnityEvent> GetFinishEvents()
        {
            return new List<UnityEvent>() { deckComponentVisualiser.onDeactivateVisualisation}; 
        }

        protected override bool CheckFinishCondition()
        {
            return true; 
        }
    }
}
