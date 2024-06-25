using System.Collections.Generic;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Card_Mechanism
{
    [CreateAssetMenu(fileName = "Starting Deck", menuName = "ScriptableObjects/Starting Deck", order = 1)]
    public class StartingDeckInfo : ScriptableObject
    {
        public List<Fraction> numbers;
    }
}
