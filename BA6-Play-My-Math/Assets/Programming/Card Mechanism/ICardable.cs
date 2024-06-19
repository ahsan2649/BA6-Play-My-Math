using System;
using Programming.Fraction_Engine;

namespace Programming.Card_Mechanism
{
    public interface ICardable {
        public String GetDisplayText();
    }

    public interface IFractionableCard : ICardable, IFractionable {
        public void Expand(int amount);
        public void Simplify(int amount);
    }

    public interface ISpecialCard : ICardable {
        
    }
}
