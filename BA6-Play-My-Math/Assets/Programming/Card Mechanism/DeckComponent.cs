using System;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;


namespace Programming.Card_Mechanism
{
    public class DeckComponent : MonoBehaviour
    {
        public static DeckComponent Instance { get; private set; }

        public UnityEvent onDeckChanged; 
        
        [HideInInspector] public List<Fraction> initDeck;
        [HideInInspector] public List<CardMovementComponent> _cardsInDeck = new();
        [SerializeField] private StartingDeckInfo startingDeck;
        [SerializeField] private GameObject numberCardPrefab;

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

            initDeck = new List<Fraction>(startingDeck.numbers); 

            FillDeckWithCards(initDeck);
            ShuffleDeck();
        }

        public void RebuildDeck()
        {
            for (var index = _cardsInDeck.Count-1; index >= 0; index--)
            {
                var cardMovementComponent = _cardsInDeck[index];
                _cardsInDeck.Remove(cardMovementComponent);
                Destroy(cardMovementComponent.gameObject);
            }
            FillDeckWithCards(initDeck);
            ShuffleDeck();
            
            onDeckChanged.Invoke();
        }

        private void FillDeckWithCards(List<Fraction> fractionList)
        {
            if (_cardsInDeck is null)
            {
                return;
            }

            _cardsInDeck.Clear();

            int listIndex = 0; 
            foreach( Fraction fraction in fractionList)
            {
                var card = Instantiate(numberCardPrefab,
                    new Vector3(transform.position.x, transform.position.y - listIndex * 0.125f, transform.position.z),
                    Quaternion.Euler(-90, 0, 0), transform);
                
                // Disabling BaseCard, so they can't be dragged from Deck
                card.GetComponentInChildren<CardMovementComponent>().enabled = false;
                card.GetComponentInChildren<NumberCardComponent>().oldValue =
                    card.GetComponentInChildren<NumberCardComponent>().Value = new Fraction(fraction);
                _cardsInDeck.Add(card.GetComponentInChildren<CardMovementComponent>());

                listIndex++; 
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
            
            onDeckChanged.Invoke();
            return pop;
        }

        public void ShuffleDeck()
        {
            _cardsInDeck.FisherYatesShuffleList(); 
        }
    }
}