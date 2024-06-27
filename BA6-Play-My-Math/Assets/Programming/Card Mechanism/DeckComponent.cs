using System.Collections.Generic;
using Programming.ScriptableObjects;
using UnityEngine;


namespace Programming.Card_Mechanism {
    public class DeckComponent : MonoBehaviour {
        [SerializeField] private GameObject numberCardPrefab;
        private List<BaseCardComponent> _cardsInDeck = new();
        [SerializeField] private StartingDeckInfo _startingDeck;
        private void Awake()
        {
            // for (int i = 0; i < 10; i++)
            // {
            //     var card = Instantiate(numberCardPrefab, new Vector3(transform.position.x, transform.position.y - i*0.125f, transform.position.z), Quaternion.Euler(-90,0,0), transform);
            //     
            //     // Disabling BaseCard, so they can't be dragged in Deck
            //     card.GetComponent<BaseCardComponent>().enabled = false;
            //     card.GetComponent<NumberCardComponent>().Value = new Fraction(Random.Range(1, 9), 1);
            //     card.GetComponent<NumberCardComponent>().oldValue =
            //         card.GetComponent<NumberCardComponent>().Value; 
            //     _cardsInDeck.Add(card.GetComponent<BaseCardComponent>());
            // }

            for (int i = 0; i < _startingDeck.numbers.Count; i++)
            {
                var card = Instantiate(numberCardPrefab, new Vector3(transform.position.x, transform.position.y - i*0.125f, transform.position.z), Quaternion.Euler(-90,0,0), transform);
                
                // Disabling BaseCard, so they can't be dragged in Deck
                card.GetComponent<BaseCardComponent>().enabled = false;
                card.GetComponent<NumberCardComponent>().Value = _startingDeck.numbers[i];
                card.GetComponent<NumberCardComponent>().oldValue =
                    card.GetComponent<NumberCardComponent>().Value; 
                _cardsInDeck.Add(card.GetComponent<BaseCardComponent>());
                
            }
            
            
        }

        public BaseCardComponent DeckPop()
        {
            if (_cardsInDeck.Count == 0)
            {
                return null;
            }
            var pop = _cardsInDeck[0];
            _cardsInDeck.Remove(pop);
            return pop;
        }
    }
}
