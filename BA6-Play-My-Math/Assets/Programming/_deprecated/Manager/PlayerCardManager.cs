using System;
using System.Collections.Generic;
using UnityEngine;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using Programming.UX_UI;

namespace Programming.Manager
{
    public class PlayerCardManager : MonoBehaviour
    {
        private List<ICardable> startingDeckICardables;
        [SerializeField] private List<NumberCard> startingFractions;
        
        [SerializeField]
        private PlayerHand playerHand;
        [SerializeField] private GameObject playerHandUI;
        [SerializeField] private GameObject numberCardUIPrefab;
        [SerializeField] private GameObject specialCardUIPrefab; 
        
        private CardDeck cardDeck;
        [SerializeField] private GameObject cardDeckUI;
        [SerializeField] private GameObject cardDeck3D;

        private CardBin cardBin;
        [SerializeField] private GameObject cardBinUI; 
        [SerializeField] private GameObject cardBin3D; 
        
        void Start()
        {
            playerHand = new PlayerHand(3);
            cardDeck = new CardDeck(CreateStartingDeck());
            cardBin = new CardBin(); 
            
            cardDeck.CreateAndShuffleRoundDeck(); 
            DrawFullHand(); 
        }

        private List<ICardable> CreateStartingDeck()
        {
            startingDeckICardables = new List<ICardable>(); 
               
            foreach (NumberCard numberCard in startingFractions)
            {
                startingDeckICardables.Add(numberCard); 
            }
            
            //temporary code for testing whether all the other stuff works
            for (int i = 1; i <= 10; i++)
            {
                NumberCard card = new NumberCard(new Fraction(i, i*2+1)); 
                startingDeckICardables.Add(card);
            }
            
            return startingDeckICardables; 
        }
        
        public void DrawFullHand()
        {
            while (playerHand._cards.Count < playerHand.handsize)
            {
                DrawCard(); 
            }
        }

        [ContextMenu("DrawCard")]
        public void DrawCard()
        {
            if (cardDeck.roundDeck.Count <= 0)
            {
                Debug.Log("no more drawable cards");
                //TODO: Check whether the game is lost
                return; 
            }
            
            ICardable card = cardDeck.PopCard(); 
            //TODO: set text of CardDeckUI
            
            playerHand.PushCard(card);
            
            GameObject newCardGameObject;
            CardDisplay cardDisplay; 
            if (card is NumberCard)
            {
                newCardGameObject = Instantiate(numberCardUIPrefab);
                cardDisplay = newCardGameObject.GetComponent<NumberCardDisplay>(); 
            }
            else if (card is SpecialCard)
            {
                newCardGameObject = Instantiate(specialCardUIPrefab);
                cardDisplay = newCardGameObject.GetComponent<SpecialCardDisplay>();
            }
            else
            {
                throw new NotImplementedException(); 
            }
            
            cardDisplay.cardBaseObject = card;
            cardDisplay.UpdateValue(); 
            cardDisplay.UpdateVisual();
            cardDisplay.name = cardDisplay.GetName();
            
            newCardGameObject.transform.SetParent(playerHandUI.transform);
            //TODO: set newCardUI Position on Screen; 
        }

        public void Discard(GameObject cardObject)
        {
            //TODO: change this access so that it's not around 2 corners
            CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>(); 
            ICardable card = cardDisplay.cardBaseObject; 
            playerHand.RemoveCard(ref card);
            cardBin.DiscardCard(card);
            
            Destroy(cardObject);
        }

        public void WinRound()
        {
            //TODO
        }

        public void AddReward()
        {
            //TODO
        }
        
        public void ReshuffleDeck()
        {
            cardDeck.CreateAndShuffleRoundDeck(); 
        }

        //TODO: Accessor Function for Adding & Removing Cards from the FullDeck & RoundDeck
        
//DEBUGGER / HELPER FUNCTIONS
        [SerializeField] private GameObject Debug_DiscardThis;

        [ContextMenu("Debug_Discard")]
        public void DiscardThis()
        {
            Discard(Debug_DiscardThis); 
        }
    }
}
