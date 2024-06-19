using System.Collections.Generic;
using Programming.Fraction_Engine;

namespace Programming.Card_Mechanism {
    public class PlayerHand {
        private List<ICardable> _cards;

        public void PushCard(ICardable newCard)
        {
            _cards.Add(newCard);
        }
    

        public void CreateFraction(ref IFractionableCard numeratorCard, ref IFractionableCard denominatorCard)
        {
            NumberCard newCard = new NumberCard(new Fraction(numeratorCard.GetValue().Numerator,
                denominatorCard.GetValue().Numerator));
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