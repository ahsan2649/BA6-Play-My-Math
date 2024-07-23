using System.Collections.Generic;
using System.Linq;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Visualisers
{
    public class DeckComponentVisualiser : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject cardCountObject; 
        [SerializeField] private TMP_Text cardCountText;
        [SerializeField] private GameObject infoCanvas;
        [SerializeField] private GameObject verticalLayoutGroup;
        [SerializeField] private float verticalLayoutGroupSizePerCard; 
        private Dictionary<Fraction, CardCountRowVisualiser> _cardCountsVisualisers = new Dictionary<Fraction, CardCountRowVisualiser>();
        private Dictionary<Fraction, int> _cardCounts = new Dictionary<Fraction, int>(); 
        [SerializeField] private GameObject cardCountRowVisualiserPrefab; 
        
        public UnityEvent onDeactivateVisualisation;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateInfoForDeckSingleton(); 
            infoCanvas.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            infoCanvas.SetActive(false);
            onDeactivateVisualisation.Invoke();
        }
        
        public void OnPointerEnter(PointerEventData eventData) 
        {
            UpdateInfoForDeckSingleton(); 
            infoCanvas.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            infoCanvas.SetActive(false);
            onDeactivateVisualisation.Invoke();
        }
        
        public void UpdateInfoForDeckSingleton()
        {
            UpdateInfo(DeckComponent.Instance, true);
        }

        public void UpdateInfoFromDeck(DeckComponent deck)
        {
            UpdateInfo(deck, infoCanvas.activeSelf);
        }
        
        private void UpdateInfo(DeckComponent deck, bool countCards = true)
        {
            if (deck is null)
            {
                Debug.Log("Trying to update DeckVisualiser without a constructed Deck");
                return; 
            }
            
            cardCountText.text = deck._cardsInDeck.Count.ToString();
            CountCards(deck._cardsInDeck, ref _cardCounts);
            WriteCountsToText(_cardCounts, ref _cardCountsVisualisers);
            OrderBySize(ref _cardCountsVisualisers);
            
            void CountCards(List<CardMovementComponent> cardDeck, ref Dictionary<Fraction, int> countHere)
            {
                countHere.Clear();
                foreach (CardMovementComponent card in cardDeck)
                {
                    NumberCardComponent numberCard = card.GetComponent<NumberCardComponent>();
                    if (numberCard is not null)
                    {
                        if (countHere.TryGetValue(numberCard.Value, out var count))
                        {
                            countHere[numberCard.Value]++; 
                        }
                        else
                        {
                            _cardCounts.Add(numberCard.Value, 1);
                        }
                    }
                } 
            }
            
            void WriteCountsToText(Dictionary<Fraction, int> values, ref Dictionary<Fraction, CardCountRowVisualiser> visualiserDictionary)
            {
                //Updating the existing visualisers
                List<Fraction> deleteTheseVisualisers = new List<Fraction>(); 
                foreach (KeyValuePair<Fraction, CardCountRowVisualiser> visualiserPair in visualiserDictionary)
                {
                    if (values.TryGetValue(visualiserPair.Key, out int count))
                    {
                        visualiserPair.Value.SetValue(count.ToString());
                        values.Remove(visualiserPair.Key); 
                    }
                    else
                    {
                        deleteTheseVisualisers.Add(visualiserPair.Key);
                    }
                }
                
                //Deleting the outdated Visualisers
                foreach (Fraction toDelete in deleteTheseVisualisers)
                {
                    Destroy(visualiserDictionary[toDelete].gameObject);
                    visualiserDictionary.Remove(toDelete); 
                }
                
                foreach (KeyValuePair<Fraction, int> countPair in values)
                {
                    GameObject newVisualiserObject = Instantiate(cardCountRowVisualiserPrefab, verticalLayoutGroup.transform);
                    CardCountRowVisualiser newCardCountRowVisualiser = newVisualiserObject.GetComponent<CardCountRowVisualiser>(); 
                    newCardCountRowVisualiser.SetDescription(countPair.Key.Numerator.ToString());
                    newCardCountRowVisualiser.SetValue(countPair.Value.ToString());
                    visualiserDictionary.Add(countPair.Key, newCardCountRowVisualiser);
                }
                RectTransform verticalTransform = verticalLayoutGroup.GetComponent<RectTransform>();
                verticalTransform.sizeDelta = new Vector2(verticalTransform.sizeDelta.x,
                    verticalLayoutGroupSizePerCard * visualiserDictionary.Count); 
            }

            void OrderBySize(ref Dictionary<Fraction, CardCountRowVisualiser> visualiserDictionary)
            {
                List<Fraction> fractions = visualiserDictionary.Keys.ToList();
                fractions = fractions.OrderBy(fraction=>fraction.Numerator).ToList();

                int index = 0;
                foreach (Fraction fraction in fractions)
                {
                    visualiserDictionary[fraction].transform.SetSiblingIndex(index);
                    index++; 
                }
            }
        }
        
        public void SetCardCountObjectActive(bool bActive)
        {
            cardCountObject.SetActive(bActive);
        }
    }
}
