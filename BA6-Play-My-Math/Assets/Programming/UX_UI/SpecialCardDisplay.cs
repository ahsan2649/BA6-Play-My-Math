using System.Collections;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.UX_UI
{
    public class SpecialCardDisplay : CardDisplay
    {
        private IFractionableCard cardBaseFraction;

        [SerializeField] private TMP_Text descriptionText;
        
        public override void UpdateVisual()
        {
            //TODO: Set Visual of cardBaseFraction
        }

        public override string GetName()
        {
            return descriptionText.text; 
        }
    }
}
