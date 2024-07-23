using System;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;

namespace Programming.Visualisers
{
    public class CardCountRowVisualiser : MonoBehaviour
    {
        [SerializeField] private TMP_Text cardDescription;
        [SerializeField] private TMP_Text cardCountText;
        
        public void SetDescription(String description)
        {
            cardDescription.text = description; 
        }

        public void SetValue(String valueText)
        {
            cardCountText.text = valueText; 
        }
    }
}
