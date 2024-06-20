using System.Collections.Generic;
using UnityEngine;

/* Includes a FullDeck which is stored between Rounds (& Rewards are added onto it)
 * and a RoundDeck, which is used for each individual battleRound (shuffled from the FullDeck at the Start of Rounds)
 */
namespace Programming.Card_Mechanism {
    public class CardDeck
    {
        public List<ICardable> FullDeck; 
        public Queue<ICardable> RoundDeck;
        
        public void InitializeDeck(List<ICardable> startingDeck)
        {
            FullDeck = new List<ICardable>(); 
            foreach (ICardable card in startingDeck)
            {
                FullDeck.Add(card);
            }
        }
        
        public ICardable CreateAndShuffleRoundDeck()
        {
            RoundDeck = new Queue<ICardable>();
            List<ICardable> FullDeckCopy = new List<ICardable>(FullDeck); 
            
            for (int i = FullDeckCopy.Count; i > 0; i++)
            {
                int RandomInt = Random.Range(0, FullDeckCopy.Count); 
                RoundDeck.Enqueue(FullDeckCopy[RandomInt]);
                FullDeckCopy.RemoveAt(RandomInt);
            }
            
            return RoundDeck.Dequeue();
        }
        
        public ICardable PopCard()
        {
            return RoundDeck.Dequeue();
        }

        public void AddCard(ICardable card)
        {
            FullDeck.Add(card);
        }

        public void RemoveCard(ICardable card)
        {
            FullDeck.Remove(card); 
        }
    }
}
