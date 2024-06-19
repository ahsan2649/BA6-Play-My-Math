using System.Collections.Generic;

namespace Programming.Card_Mechanism {
    public class CardDeck {
        public Queue<ICardable> Cards;

        public void InitializeDeck(List<ICardable> startingDeck)
        {
            foreach (ICardable card in startingDeck)
            {
                Cards.Enqueue(card);
            }
        }
    
        public ICardable PopCard()
        {
            return Cards.Dequeue();
        }
    }
}
