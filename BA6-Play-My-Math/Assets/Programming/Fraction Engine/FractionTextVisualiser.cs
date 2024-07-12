using System;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;

namespace Programming.Visualisers
{
    public class FractionTextVisualiser : MonoBehaviour
    {
        private Fraction _fraction;

        [SerializeField] private GameObject wholeNumberGameObject; 
        [SerializeField] private TMP_Text wholeNumber;

        [SerializeField] private GameObject fractionGameObject; 
        [SerializeField] private TMP_Text numerator;
        [SerializeField] private TMP_Text denominator; 
        
        public void SetFraction(Fraction fraction)
        {
            _fraction = fraction; 
            if (fraction is null)
            {
                wholeNumber.text = ""; 
                numerator.text = "";
                denominator.text = ""; 
            }
            else
            {
                wholeNumber.text = fraction.Numerator.ToString(); 
                numerator.text = fraction.Numerator.ToString();
                denominator.text = fraction.Denominator.ToString();
                if (fraction.Denominator == 1)
                {
                    wholeNumberGameObject.SetActive(true);
                    fractionGameObject.SetActive(false);    
                }
                else
                {
                    wholeNumberGameObject.SetActive(false);
                    fractionGameObject.SetActive(true);    
                }
            }
        }

        public Fraction GetFraction()
        {
            if (_fraction is null)
            {
                return null; 
            }
            else
            {
                return _fraction; 
            }
        }

        public void DisplayDecimals(float numerator, float denominator)
        {
            //TODO @Phyvie: 
        }
        
        private void OnValidate()
        {
            if (numerator is null || denominator is null)
            {
                Debug.LogWarning("FractionTextVisualiser not all references set");
                return; 
            }
            
            if (numerator.text is not null && int.TryParse(numerator.text, out int outNumerator) && 
                denominator.text is not null && int.TryParse(denominator.text, out int outDenominator))
            {
                _fraction = new Fraction(outNumerator, outDenominator); 
            }
        }
    }
}