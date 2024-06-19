using System.Collections.Generic;

namespace Programming.Card_Mechanism {
    public class CardBin {
        private List<ICardable> _disCards;

        public void DiscardCard(ICardable card)
        {
            _disCards.Add(card);
        }
    }
}
