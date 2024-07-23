using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Visualisers;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public class TutorialVisualCardsInDeck: TutorialVisualElement
    {
        private int timesCardsInDeckChecked = 0; 
        
        public override void Start()
        {
            base.Start();
        }
        
        protected override List<UnityEvent> GetCloseEvents()
        {
            DeckComponentVisualiser dCV = DeckComponent.Instance.gameObject.GetComponent<DeckComponentVisualiser>(); 
            return new List<UnityEvent>() { dCV.onDeactivateVisualisation }; 
        }

        protected override bool CheckCloseCondition()
        {
            timesCardsInDeckChecked++;
            if (timesCardsInDeckChecked >= 2)
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }
    }
}
