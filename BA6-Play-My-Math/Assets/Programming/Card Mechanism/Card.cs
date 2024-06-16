using Programming.Fraction_Engine;

namespace Programming.Card_Mechanism
{
    public class Card : ICardable
    {
        public Card(Fraction value)
        {
            _value = value;
        }

        private Fraction _value;
        public Fraction GetValue()
        {
            return _value;
        }

        public Fraction SetValue(Fraction newValue)
        {
            _value = newValue;
            return _value;
        }
    }
}