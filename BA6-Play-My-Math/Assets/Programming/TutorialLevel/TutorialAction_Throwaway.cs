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
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/LearnThrowaway")]
    public class TutorialAction_Throwaway : TutorialActionInfo
    {
        public override void FinishLevel(TutorialLevelInfo levelInfo)
        {
            for (int i = 0; i < 6; i++)
            {
                DeckComponent.Instance.AddCardToDeck(new Fraction(4, 1), false, true);
            }
        }
        
        public override void StartLevel(TutorialLevelInfo levelInfo)
        {
            base.StartLevel(levelInfo);
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualThrowaway>();
            DeckComponent.Instance._cardsInDeck[0].GetComponent<NumberCardComponent>().Value = new Fraction(4, 1); 
            DeckComponent.Instance._cardsInDeck[6].GetComponent<NumberCardComponent>().Value = new Fraction(4, 1); 
            DeckComponent.Instance._cardsInDeck[9].GetComponent<NumberCardComponent>().Value = new Fraction(4, 1); 
        }

        public override void UpdateLevel(TutorialLevelInfo levelInfo)
        {
            if (DeckComponent.Instance._cardsInDeck.Count < 5)
            {
                foreach (Fraction fraction in new List<Fraction>()
                         {
                             new Fraction(1, 1), 
                             new Fraction(2, 1), 
                             new Fraction(3, 1), 
                             new Fraction(4, 1) 
                         }.FisherYatesShuffle())
                {
                    DeckComponent.Instance.AddCardToDeck(fraction, true, false);
                }
            }
        }
    }
}
