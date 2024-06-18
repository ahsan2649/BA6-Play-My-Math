using Programming.Fraction_Engine;

namespace Programming.Card_Mechanism
{
    public interface ICardable : IFractionable {
        public void Expand(int amount);
        public void Simplify(int amount);
    }
}
