using System.Collections.Generic;
using UnityEngine;

/* Includes a FullDeck which is stored between Rounds (& Rewards are added onto it)
 * and a RoundDeck, which is used for each individual battleRound (shuffled from the FullDeck at the Start of Rounds)
 */
namespace Programming.Card_Mechanism {
    public class CardDeck
    {
        public List<ICardable> fullDeck; 
        public Queue<ICardable> roundDeck;

        public CardDeck(List<ICardable> startingDeck)
        {
            InitializeDeck(startingDeck);
        }
        
        public void InitializeDeck(List<ICardable> startingDeck)
        {
            fullDeck = new List<ICardable>(); 
            foreach (ICardable card in startingDeck)
            {
                fullDeck.Add(card);
            }
        }
        
        public ICardable CreateAndShuffleRoundDeck()
        {
            roundDeck = new Queue<ICardable>();
            List<ICardable> FullDeckCopy = new List<ICardable>(fullDeck); 
            
            for (int i = FullDeckCopy.Count; i > 0; i--)
            {
                int RandomInt = Random.Range(0, FullDeckCopy.Count); 
                roundDeck.Enqueue(FullDeckCopy[RandomInt]);
                FullDeckCopy.RemoveAt(RandomInt);
            }
            
            return roundDeck.Dequeue();
        }
        
        public ICardable PopCard()
        {
            return roundDeck.Dequeue();
        }

        public void AddCard(ICardable card)
        {
            fullDeck.Add(card);
        }

        public void RemoveCard(ICardable card)
        {
            fullDeck.Remove(card); 
        }
    }
}
