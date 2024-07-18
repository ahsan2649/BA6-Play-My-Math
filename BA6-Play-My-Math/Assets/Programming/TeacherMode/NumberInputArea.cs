using System.Runtime.CompilerServices;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;

namespace Programming.TeacherMode
{
    public class NumberInputArea : MonoBehaviour
    {
        public static NumberInputArea Instance; 
        private FractionChanger _fractionChanger;
        private Fraction Fraction => _fractionChanger.GetChangeableFraction(); 
        private int Numerator => _fractionChanger.GetChangeableFraction().Numerator; 
        private int Denominator => _fractionChanger.GetChangeableFraction().Denominator;
        
        private int _unchangedNumber; 
        private int _storedNumber; 
        
        public void Awake()
        {
            this.MakeSingleton(ref Instance);
            gameObject.SetActive(false);
        }
        
        public void InputDigit(int newDigit)
        {
            _storedNumber = _storedNumber * 10 + newDigit; 
            _fractionChanger.SetText(_storedNumber);
        }

        public void DeleteDigit()
        {
            _storedNumber /= 10;
            _fractionChanger.SetText(_storedNumber);
        }

        public void OpenInputArea(FractionChanger fractionChanger, TMP_Text textField)
        {
            _fractionChanger = fractionChanger;
            gameObject.SetActive(true);
            _unchangedNumber = _storedNumber = int.Parse(textField.text);
        }
        
        public void CloseInputArea()
        {
            _fractionChanger?.SetText(_unchangedNumber);
            _fractionChanger = null;
            gameObject.SetActive(false);
        }

        public void InputDone()
        {
            _unchangedNumber = _storedNumber; 
            _fractionChanger.FinaliseInput();
        }
    }
}
