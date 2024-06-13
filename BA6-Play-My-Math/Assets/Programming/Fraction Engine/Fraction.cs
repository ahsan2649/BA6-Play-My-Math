using System;

namespace Programming.Fraction_Engine
{
    public class Fraction
    {
        public readonly int Numerator;
        public readonly int Denominator;


        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));
            }
        
            this.Numerator = numerator;
            this.Denominator = denominator;
        }


        #region Overrides

        public override string ToString()
        {
            return $"{Numerator} / {Denominator}";
        }
    
        protected bool Equals(Fraction other)
        {
            return this == other;
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
            return HashCode.Combine(Numerator, Denominator);
        }

        #endregion
    
        #region Operators

        // Equality
        public static bool operator ==(Fraction a, Fraction b)
        {
            return 
                a.LowestTerm().Numerator == b.LowestTerm().Numerator &&
                a.LowestTerm().Denominator == b.LowestTerm().Denominator;
        }

        // Inequality
        public static bool operator !=(Fraction a, Fraction b)
        {
            return 
                a.LowestTerm().Numerator != b.LowestTerm().Numerator ||
                a.LowestTerm().Denominator != b.LowestTerm().Denominator;
        }

        // LessThan
        public static bool operator <(Fraction a, Fraction b)
        {
            int lcm = LeastCommonMultiple(a.Denominator, b.Denominator);
            return a.Numerator * (lcm / a.Denominator) < b.Numerator * (lcm / b.Denominator);
        }

        // GreaterThan
        public static bool operator >(Fraction a, Fraction b)
        { 
            int lcm = LeastCommonMultiple(a.Denominator, b.Denominator);
            return a.Numerator * (lcm / a.Denominator) > b.Numerator * (lcm / b.Denominator);
        }

        // Positive
        public static Fraction operator +(Fraction a) => a;
    
        // Negative
        public static Fraction operator -(Fraction a) => new Fraction(-a.Numerator, a.Denominator);

        // Add
        public static Fraction operator +(Fraction a, Fraction b)
        {
            int lcm = LeastCommonMultiple(a.Denominator, b.Denominator);

            return new Fraction(
                a.Numerator * (lcm / a.Denominator) +
                b.Numerator * (lcm / b.Denominator),
                lcm
            );
        }
    
        // Subtract (Negative Add)
        public static Fraction operator -(Fraction a, Fraction b)
            => a + (-b);
    
        // Multiply
        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
    
        // Divide
        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b.Numerator == 0)
            {
                throw new DivideByZeroException();
            }
            return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }

        #endregion

        #region Helpers

        public Fraction LowestTerm()
        {
            int gcd = GreatestCommonDivisor(Numerator, Denominator);
            return new Fraction(Numerator / gcd, Denominator / gcd);
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
