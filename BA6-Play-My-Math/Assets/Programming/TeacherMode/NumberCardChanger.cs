using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using Programming.OverarchingFunctionality;
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
        
        [SerializeField] private GameObject fractionButtonsHolder; 
        [SerializeField] private GameObject wholeButtonHolder;

        public override void OnPointerClick(PointerEventData eventData)
        {
            //actually don't turn anything active
        }

        public override void CheckEnable()
        {
            if (SceneManaging.Instance.bTeacherMode)
            {
                fractionButtonsHolder.SetActive(true);
                wholeButtonHolder.SetActive(true);
            }
            else
            {
                fractionButtonsHolder.SetActive(false);
                wholeButtonHolder.SetActive(false);
                Destroy(this);
            }
        }

        public void SetChangeButtonsForValue()
        {
            NumberCardComponent number = GetComponent<NumberCardComponent>();
            if (number.IsFraction)
            {
                fractionButtonsHolder.SetActive(true);
                wholeButtonHolder.SetActive(false);
            }
            else
            {
                fractionButtonsHolder.SetActive(false);
                wholeButtonHolder.SetActive(true);
            }
            UpdateTexts();
        }
        
        public override void FinaliseInput()
        {
            if (numeratorText.IsActive() || denominatorText.IsActive())
            {
                numberCardComponent.Value = new Fraction(int.Parse(numeratorText.text), int.Parse(denominatorText.text)); 
            }
            else
            {
                numberCardComponent.Value = new Fraction(int.Parse(wholeNumberText.text), 1); 
            }
        }
        
        public override Fraction GetChangeableFraction()
        {
            return numberCardComponent.Value; 
        }

        public void UpdateTexts()
        {
            wholeNumberText.text = numberCardComponent.Value.Numerator.ToString(); 
            numeratorText.text = numberCardComponent.Value.Numerator.ToString(); 
            denominatorText.text = numberCardComponent.Value.Denominator.ToString(); 
        }
    }
}
