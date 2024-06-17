using Programming.Fraction_Engine;

namespace Programming.Card_Mechanism
{
    public class Card : ICardable
    {
        private Fraction _value;
        public Card(Fraction value)
        {
            _value = value;
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
    }
}