using System.Collections.Generic;
using UnityEngine.PlayerLoop;

namespace Programming.Card_Mechanism {
    public class CardBin {
        private List<ICardable> _disCards;

        public CardBin()
        {
            _disCards = new List<ICardable>(); 
        }
        
        public void DiscardCard(ICardable card)
        {
            _disCards.Add(card);
        }
    }
}
