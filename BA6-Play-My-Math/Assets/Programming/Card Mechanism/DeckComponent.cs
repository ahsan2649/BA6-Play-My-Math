using System;
using System.Collections.Generic;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;


namespace Programming.Card_Mechanism
{
    public class DeckComponent : MonoBehaviour
    {
        public List<BaseCardComponent> _cardsInDeck = new();
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
        }

        private void Start()
        {
            if (startingDeck == null)
            {
                Debug.LogError("No Starting Deck!");
                return;
            }
            for (int i = 0; i < startingDeck.numbers.Count; i++)
            {
                var card = Instantiate(numberCardPrefab,
                    new Vector3(transform.position.x, transform.position.y - i * 0.125f, transform.position.z),
                    Quaternion.Euler(-90, 0, 0), transform);

                // Disabling BaseCard, so they can't be dragged in Deck
                card.GetComponent<BaseCardComponent>().enabled = false;
                card.GetComponent<NumberCardComponent>().oldValue =
                    card.GetComponent<NumberCardComponent>().Value = new Fraction(startingDeck.numbers[i]);
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