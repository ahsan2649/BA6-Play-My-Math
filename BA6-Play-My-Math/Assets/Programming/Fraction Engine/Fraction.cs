using System;

namespace Programming.Fraction_Engine
{
    public class Fraction
    {
    

        private readonly int _numerator;
        private readonly int _denominator;


        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));
            }
        
            this._numerator = numerator;
            this._denominator = denominator;
        }


        #region Overrides

        public override string ToString()
        {
            return $"{_numerator} / {_denominator}";
        }
    
        protected bool Equals(Fraction other)
        {
            return _numerator == other._numerator && _denominator == other._denominator;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Fraction)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_numerator, _denominator);
        }

        #endregion
    
        #region Operators

        // Equality
        public static bool operator ==(Fraction a, Fraction b)
        {
            return 
                a.LowestTerm()._numerator == b.LowestTerm()._numerator &&
                a.LowestTerm()._denominator == b.LowestTerm()._denominator;
        }

        // Inequality
        public static bool operator !=(Fraction a, Fraction b)
        {
            return 
                a.LowestTerm()._numerator != b.LowestTerm()._numerator ||
                a.LowestTerm()._denominator != b.LowestTerm()._denominator;
        }

        // LessThan
        public static bool operator <(Fraction a, Fraction b)
        {
            int lcm = LeastCommonMultiple(a._denominator, b._denominator);
            return a._numerator * (lcm / a._denominator) < b._numerator * (lcm / b._denominator);
        }

        // GreaterThan
        public static bool operator >(Fraction a, Fraction b)
        { 
            int lcm = LeastCommonMultiple(a._denominator, b._denominator);
            return a._numerator * (lcm / a._denominator) > b._numerator * (lcm / b._denominator);
        }

        // Positive
        public static Fraction operator +(Fraction a) => a;
    
        // Negative
        public static Fraction operator -(Fraction a) => new Fraction(-a._numerator, a._denominator);

        // Add
        public static Fraction operator +(Fraction a, Fraction b)
        {
            int lcm = LeastCommonMultiple(a._denominator, b._denominator);

            return new Fraction(
                a._numerator * (b._denominator / lcm) +
                b._numerator * (a._denominator / lcm),
                lcm
            );
        }
    
        // Subtract (Negative Add)
        public static Fraction operator -(Fraction a, Fraction b)
            => a + (-b);
    
        // Multiply
        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a._numerator * b._numerator, a._denominator * b._denominator);
    
        // Divide
        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b._numerator == 0)
            {
                throw new DivideByZeroException();
            }
            return new Fraction(a._numerator * b._denominator, a._denominator * b._numerator);
        }

        #endregion

        #region Helpers

        private Fraction LowestTerm()
        {
            int gcd = GreatestCommonDivisor(_numerator, _denominator);
            return new Fraction(_numerator / gcd, _denominator / gcd);
        }
        static int GreatestCommonDivisor(int a, int b) 
        { 
            if (a == 0) 
                return b;  
            return GreatestCommonDivisor(b % a, a);  
        } 
        static int LeastCommonMultiple(int a, int b) 
        { 
            return (a / GreatestCommonDivisor(a, b)) * b; 
        } 

        #endregion


    }
}
