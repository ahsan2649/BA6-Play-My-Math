using System;
using Programming.Fraction_Engine;

namespace Programming.Card_Mechanism
{
    [Serializable]
    public class NumberCard : IFractionableCard
    {
        private Fraction _value;
        
        public NumberCard(Fraction value)
        {
            _value = value;
        }

        public bool IsFraction()
        {
            if(GetValue().Numerator % GetValue().Denominator == 0)
            {
                return false; 
            }
            else { return true; }
        }

        public Fraction GetValue()
        {
            return _value;
        }

        public Fraction SetValue(Fraction newValue)
        {
            _value = newValue;
            return _value;
        }


        public void Expand(int amount)
        {
            SetValue(GetValue().ExpandBy(amount));
        }

        public void Simplify(int amount)
        {
            SetValue(GetValue().SimplifyBy(amount));
            
        }

        public string GetDisplayText()
        {
            return GetValue().ToString();
        }
    }
}