using System;
using System.Collections.Generic;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;


namespace Programming.Card_Mechanism
{
    public class DeckComponent : MonoBehaviour
    {
        public List<CardMovementComponent> _cardsInDeck = new();
        [SerializeField] private StartingDeckInfo startingDeck;
        [SerializeField] private GameObject numberCardPrefab;
        public static DeckComponent Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            FillDeckWithCards(startingDeck.numbers);
        }

        private void FillDeckWithCards(List<Fraction> fractionList)
        {
            if (startingDeck == null)
            {
                Debug.LogError("No Starting Deck!");
                return;
            }
                
            for (int i = 0; i < fractionList.Count; i++)
            {
                var card = Instantiate(numberCardPrefab,
                    new Vector3(transform.position.x, transform.position.y - i * 0.125f, transform.position.z),
                    Quaternion.Euler(-90, 0, 0), transform);

                // Disabling BaseCard, so they can't be dragged from Deck
                card.GetComponentInChildren<CardMovementComponent>().enabled = false;
                card.GetComponentInChildren<NumberCardComponent>().oldValue =
                    card.GetComponentInChildren<NumberCardComponent>().Value = new Fraction(startingDeck.numbers[i]);
                _cardsInDeck.Add(card.GetComponentInChildren<CardMovementComponent>());
            }
        }
        
        public CardMovementComponent DeckPop()
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