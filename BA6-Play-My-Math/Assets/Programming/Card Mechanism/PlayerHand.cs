using System.Collections.Generic;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Card_Mechanism {
    [System.Serializable] //TODO?: idk whether this should be serialised, but it's useful in order to see in the inspector whether stuff works properly
    public class PlayerHand { 
        public List<ICardable> _cards; //TODO: Also not visible in Inspector :(
        public int handsize = 2;

        public PlayerHand(int handsize)
        {
            this.handsize = handsize;
            _cards = new List<ICardable>(); 
        }
        
        public void PushCard(ICardable newCard) 
        {
            _cards.Add(newCard);
        }
    
        public void CreateFraction(ref IFractionableCard numeratorCard, ref IFractionableCard denominatorCard)
        {
            NumberCard newCard = new NumberCard(new Fraction(numeratorCard.GetValue().Numerator,
                denominatorCard.GetValue().Numerator));
            if (!newCard.IsFraction())
            {
                Fraction fraction = new Fraction(newCard.GetValue().Numerator / newCard.GetValue().Denominator, 1);
                newCard.SetValue(fraction);
            }
            _cards.Remove(numeratorCard);
            _cards.Remove(denominatorCard);
            _cards.Add(newCard);
        }

        public List<IFractionableCard> GetFractionables()
        {
            List<IFractionableCard> highlightedCards = new List<IFractionableCard>();
            foreach (ICardable card in _cards)
            {
                if (card is IFractionableCard && (card as IFractionableCard).GetValue().Denominator == 1)
                {
                    highlightedCards.Add(card as IFractionableCard);
                }
            }

            return highlightedCards;
        }

        public List<IFractionableCard> GetAttackables(Fraction enemyValue)
        {
            List<IFractionableCard> attackableCards = new List<IFractionableCard>();
            foreach (ICardable card in _cards)
            {
                if (card is IFractionableCard && (card as IFractionableCard).GetValue() == enemyValue)
                {
                    attackableCards.Add(card as IFractionableCard);
                }
            }

            return attackableCards;
        }

        public ICardable RemoveCard(ref ICardable card)
        {
            _cards.Remove(card);
            return card;
        }
    }
}