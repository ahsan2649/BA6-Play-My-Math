using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.TeacherMode
{
    public class NumberCardChanger : FractionChanger
    {
        [SerializeField] private NumberCardComponent numberCardComponent;

        [SerializeField] private TMP_Text numeratorText; 
        [SerializeField] private TMP_Text denominatorText;
        [SerializeField] private TMP_Text wholeNumberText; 
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            
        }

        public override void FinaliseInput()
        {
            numberCardComponent.Value = new Fraction(int.Parse(numeratorText.text), int.Parse(denominatorText.text)); 
        }
        
        public override Fraction GetChangeableFraction()
        {
            return numberCardComponent.Value; 
        }
    }
}
