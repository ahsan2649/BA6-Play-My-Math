using System;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Card_Mechanism
{
    [Serializable]
    public class SpecialCard : ICardable
    {
        [SerializeField] private String descriptionText;
        private Action playAction; 
        
        public SpecialCard(Action playAction)
        {
            this.playAction = playAction; 
        }

        public string GetDisplayText()
        {
            return descriptionText;
        }
    }
}