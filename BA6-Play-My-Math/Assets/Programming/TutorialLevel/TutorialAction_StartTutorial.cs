using System.Collections.Generic;
using System.Linq;
using Programming.Card_Mechanism;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using Programming.TutorialVisual;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/StartTutorial")]
    public class TutorialLevelFirst : TutorialActionInfo
    {
        private List<Fraction> extraCards = new List<Fraction>()
        {
            new Fraction(1, 1),
            new Fraction(2, 1),
            new Fraction(3, 1)
        }; 
        
        public override void InitialiseLevel(TutorialLevelInfo levelInfo)
        {
            base.StartLevel(levelInfo);
            SetupInitDeck();
            PlayerHandComponent.Instance.FillHand();

            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualDragCard>(); 
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualPlayCard>(); 
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualThrowaway>();
        }
        
        public override void StartLevel(TutorialLevelInfo levelInfo)
        {
            
        }

        public override void UpdateLevel(TutorialLevelInfo levelInfo)
        { 
            if (DeckComponent.Instance._cardsInDeck.Count < 5 && DeckComponent.Instance._cardsInDeck.Count != 0)
            {
                foreach (Fraction fraction in extraCards.FisherYatesShuffle())
                {
                    DeckComponent.Instance.AddCardToDeck(fraction, true, false);
                }
            }
        }
        
        public override void FinishLevel(TutorialLevelInfo levelInfo)
        {
            base.FinishLevel(levelInfo);
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualSelectReward>();
        }
        
        public void SetupInitDeck()
        {
            DeckComponent deck = DeckComponent.Instance;

            List<Fraction> startingDeck = new List<Fraction>()
            {
                new Fraction(1, 1),
                new Fraction(3, 1), 
                new Fraction(2, 1), 
            };
            for (int i = 0; i < 5; i++)
            {
                startingDeck = startingDeck.Concat(new List<Fraction>()
                {
                    new Fraction(1, 1),
                    new Fraction(2, 1),
                    new Fraction(3, 1)
                }.FisherYatesShuffle()).ToList(); 
            }

            foreach (Fraction fraction in startingDeck)
            {
                deck.AddCardToDeck(fraction, true, true);
            }
        }
    }
}
