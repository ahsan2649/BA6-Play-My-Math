using System.Collections.Generic;
using Programming.Card_Mechanism;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Programming.Visualisers
{
    public class DeckComponentVisualiser : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text cardCountText;
        [SerializeField] private GameObject infoCanvas; 
        [SerializeField] private CardCountRowVisualiser[] countOfCards;
        [SerializeField] private CardCountRowVisualiser higherCardsCountText; 
        [SerializeField] private CardCountRowVisualiser specialCardCountText; 
        
        public void OnPointerClick(PointerEventData eventData)
        {
            infoCanvas.SetActive(!infoCanvas.activeSelf);
        }
        
        public void UpdateInfoForDeckSingleton()
        {
            UpdateInfo(DeckComponent.Instance, infoCanvas.activeSelf);
        }

        public void UpdateInfoFromDeck(DeckComponent deck)
        {
            UpdateInfo(deck, infoCanvas.activeSelf);
        }
        
        private void SetupInfoArea()
        {
            for (int i = 0; i < countOfCards.Length; i++)
            {
                countOfCards[i].SetDescription(i.ToString()); 
            }
        }
        
        private void UpdateInfo(DeckComponent deck, bool countCards = true)
        {
            cardCountText.text = deck._cardsInDeck.Count.ToString();

            if (countCards)
            {
                int[] cardCounts = CountCards(deck._cardsInDeck, countOfCards.Length, out int higherCardsCount, out int specialCardCount);
                WriteCountsToText(cardCounts, ref higherCardsCountText, ref specialCardCountText, in higherCardsCount, in specialCardCount); 
            }

            int[] CountCards(List<CardMovementComponent> cardDeck, int maxNumber, out int higherCardsCount, out int extraCardCount)
            {
                int[] cardCount = new int[maxNumber+1];
                higherCardsCount = 0;
                extraCardCount = 0; 
                
                foreach (CardMovementComponent card in cardDeck)
                {
                    NumberCardComponent numberCard = card.GetComponent<NumberCardComponent>();
                    if (numberCard is not null)
                    {
                        if (numberCard.Value.Numerator <= maxNumber)
                        {
                            cardCount[numberCard.Value.Numerator] += 1; 
                        }
                        else
                        {
                            higherCardsCount++; 
                        }
                    }
                    else
                    {
                        extraCardCount++; 
                    }
                }

                return cardCount; 
            }
            
            void WriteCountsToText(int[] cardCounts, ref CardCountRowVisualiser higherCardsCountText, ref CardCountRowVisualiser specialCardsCountText, in int higherCardsCount, in int specialCardsCount)
            {
                for (int i = 0; i < countOfCards.Length; i++)
                {
                    countOfCards[i].SetValue(cardCounts[i].ToString()); 
                }

                higherCardsCountText.SetValue(higherCardsCount.ToString()); 
                specialCardsCountText.SetValue(specialCardsCount.ToString());
            }
        }
    }
}
