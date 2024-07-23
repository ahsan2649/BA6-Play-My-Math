using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Visualisers;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public class TutorialVisualCardsInDeck: TutorialVisualElement
    {
        [SerializeField] private DeckComponentVisualiser deckComponentVisualiser;

        public void Start()
        {
            DeckComponent.Instance.GetComponent<DeckComponentVisualiser>()?.SetCardCountObjectActive(true);
        }
        
        protected override List<UnityEvent> GetCloseEvents()
        {
            return new List<UnityEvent>() { deckComponentVisualiser.onDeactivateVisualisation}; 
        }

        protected override bool CheckCloseCondition()
        {
            return true; 
        }
    }
}
