using System;
using System.Collections;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.OverarchingFunctionality;
using Programming.ScriptableObjects;
using Programming.Visualisers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = System.Random;


namespace Programming.Card_Mechanism
{
    public class DeckComponent : MonoBehaviour
    {
        public static DeckComponent Instance { get; private set; }

        public UnityEvent onDeckChanged;

        [HideInInspector] private List<Fraction> initDeck = new List<Fraction>();
         public List<CardMovementComponent> _cardsInDeck = new();
        [SerializeField] private StartingDeckInfo startingDeck;
        public GameObject numberCardPrefab;
        private bool ready;

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
        }

        public void CreateDeck()
        {
            initDeck = GetStartingDeck().numbers;

            FillDeckWithCards(initDeck);
            ShuffleDeck();
        }

        public void RebuildDeckAndShuffle()
        {
            RebuildDeck(true);
        }
        
        public void RebuildDeck(bool bShuffle = true)
        {
            for (var index = _cardsInDeck.Count - 1; index >= 0; index--)
            {
                var cardMovementComponent = _cardsInDeck[index];
                _cardsInDeck.Remove(cardMovementComponent);
                Destroy(cardMovementComponent.gameObject);
            }

            FillDeckWithCards(initDeck);
            if (bShuffle)
            {
                ShuffleDeck();
            }

            onDeckChanged.Invoke();
        }

        private void FillDeckWithCards(List<Fraction> fractionList)
        {
            if (_cardsInDeck is null)
            {
                return;
            }

            _cardsInDeck.Clear();

            foreach (Fraction fraction in fractionList)
            {
                AddCardToPlayingDeck(fraction);
            }
        }

        public void AddCardToDeck(Fraction fraction, bool addToPlayDeck, bool addToInitDeck)
        {
            if (addToInitDeck)
            {
                initDeck.Add(new Fraction(fraction));
            }

            if (addToPlayDeck)
            {
                AddCardToPlayingDeck(fraction);
            }
        }
        
        private void AddCardToPlayingDeck(Fraction fraction)
        {
            var card = Instantiate(numberCardPrefab,
                new Vector3(transform.position.x, transform.position.y - _cardsInDeck.Count * 0.125f, transform.position.z),
                transform.rotation, transform);

            // Disabling BaseCard, so they can't be dragged from Deck
            card.GetComponentInChildren<CardMovementComponent>().enabled = false;
            NumberCardComponent numberCard = card.GetComponentInChildren<NumberCardComponent>(); 
            numberCard.oldValue = numberCard.Value = new Fraction(fraction);
            numberCard.fractionTextVisualiser.SetInDeck(true);
            _cardsInDeck.Add(card.GetComponentInChildren<CardMovementComponent>());
        }

        public CardMovementComponent DeckPop()
        {
            if (_cardsInDeck.Count == 0)
            {
                return null;
            }

            var pop = _cardsInDeck[0];
            _cardsInDeck.Remove(pop);
            NumberCardComponent nC = pop.GetComponent<NumberCardComponent>();
            nC.IsFractionPreview = false; 

            onDeckChanged.Invoke();
            return pop;
        }

        public void ShuffleDeck()
        {
            _cardsInDeck.FisherYatesShuffle();
        }

        public void WinRebuildDeck()
        {
            if (!ready)
            {
                ready = true;
                return;
            }

            ready = false;
            RebuildDeckAndShuffle();
        }
        
        #region StartingDeck

        /// <summary>
        /// Returns a starting deck based on the selected GameMode
        /// </summary>
        /// <returns></returns>
        public StartingDeckInfo GetStartingDeck()
        {
            StartingDeckInfo newStartingDeckInfo = new StartingDeckInfo();
            switch (SceneManaging.gameMode)
            {
                case (SceneManaging.GameMode.easy23):
                    newStartingDeckInfo.numbers = new List<Fraction>()
                    {
                        new Fraction(4,1),
                        new Fraction(4,1),
                        new Fraction(6,1),
                        new Fraction(6,1),
                        new Fraction(8,1),
                        new Fraction(8,1),
                        new Fraction(9,1),
                        new Fraction(9,1),
                        new Fraction(3,1),
                        new Fraction(12,1),
                    };
                    break;
                case (SceneManaging.GameMode.medium235):
                    newStartingDeckInfo.numbers = new List<Fraction>()
                    {
                        new Fraction(4,1),
                        new Fraction(4,1),
                        new Fraction(6,1),
                        new Fraction(6,1),
                        new Fraction(8,1),
                        new Fraction(8,1),
                        new Fraction(9,1),
                        new Fraction(9,1),
                        new Fraction(3,1),
                        new Fraction(12,1),
                        new Fraction(5,1),
                        new Fraction(5,1),
                        new Fraction(10,1),
                        new Fraction(10,1),
                    };
                    break;
                case (SceneManaging.GameMode.hard2357):
                    newStartingDeckInfo.numbers = new List<Fraction>()
                    {
                        new Fraction(4,1),
                        new Fraction(4,1),
                        new Fraction(6,1),
                        new Fraction(6,1),
                        new Fraction(8,1),
                        new Fraction(8,1),
                        new Fraction(9,1),
                        new Fraction(9,1),
                        new Fraction(3,1),
                        new Fraction(12,1),
                        new Fraction(5,1),
                        new Fraction(5,1),
                        new Fraction(10,1),
                        new Fraction(10,1),
                        new Fraction(7,1),
                        new Fraction(7,1),
                        new Fraction(14,1),
                        new Fraction(14,1),
                    };
                    break;
                case (SceneManaging.GameMode.easyAdditionSmallNumbers):
                    newStartingDeckInfo.numbers = new List<Fraction>()
                    {
                        new Fraction(1,1),
                        new Fraction(1,1),
                        new Fraction(2,1),
                        new Fraction(2,1),
                        new Fraction(4,1),
                        new Fraction(4,1),
                        new Fraction(8,1),
                        new Fraction(8,1),
                    };
                    break;
                case (SceneManaging.GameMode.mediumAddition):
                    newStartingDeckInfo.numbers = new List<Fraction>()
                    {
                        new Fraction(1,1),
                        new Fraction(2,1),
                        new Fraction(3,1),
                        new Fraction(4,1),
                        new Fraction(4,1),
                        new Fraction(6,1),
                        new Fraction(6,1),
                        new Fraction(8,1),
                        new Fraction(8,1),
                        new Fraction(9,1),
                        new Fraction(9,1),
                    };
                    break;
                case (SceneManaging.GameMode.multiplicationOnly):
                    newStartingDeckInfo.numbers = new List<Fraction>()
                    {
                        new Fraction(4,1),
                        new Fraction(4,1),
                        new Fraction(6,1),
                        new Fraction(6,1),
                        new Fraction(8,1),
                        new Fraction(8,1),
                        new Fraction(9,1),
                        new Fraction(9,1),
                        new Fraction(12,1),
                        new Fraction(18,1),
                    };
                    break;
                default:
                    newStartingDeckInfo.numbers = new List<Fraction>();
                    break; 
            }
            return newStartingDeckInfo;
        }

        #endregion

    }
}