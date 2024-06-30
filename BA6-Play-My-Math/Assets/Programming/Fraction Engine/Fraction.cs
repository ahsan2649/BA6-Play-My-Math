using System;
using UnityEngine;

namespace Programming.Fraction_Engine
{
    [Serializable]
    public class Fraction
    {
        public int Numerator
        {
            get => numerator;
            private set => numerator = value;
        }
        [SerializeField] private int numerator;
        
        public int Denominator
        {
            get => denominator;
            private set => denominator = value;
        }
        [SerializeField] private int denominator = 1;


        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new ArgumentException("Denominator cannot be zero.", nameof(denominator));
            }

            this.numerator = numerator;
            this.denominator = denominator;
        }

        public bool IsWhole()
        {
            return denominator == 1;
        }

        public bool IsOne()
        {
            return numerator == 1 && denominator == 1;
        }

        public bool IsBetween(float a, float b)
        {
            return (float) numerator / denominator >= a && (float) numerator / denominator <= b; 
        }

        #region Overrides

        public override string ToString()
        {
            return $"{numerator} / {denominator}";
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
            return HashCode.Combine(numerator, denominator);
        }

        #endregion
    
        #region Operators

        // Equality
        public static bool operator ==(Fraction a, Fraction b)
        {
            return 
                a.LowestTerm().numerator == b.LowestTerm().numerator &&
                a.LowestTerm().denominator == b.LowestTerm().denominator;
        }

        // Inequality
        public static bool operator !=(Fraction a, Fraction b)
        {
            return 
                a.LowestTerm().numerator != b.LowestTerm().numerator ||
                a.LowestTerm().denominator != b.LowestTerm().denominator;
        }

        // LessThan
        public static bool operator <(Fraction a, Fraction b)
        {
            int lcm = LeastCommonMultiple(a.denominator, b.denominator);
            return a.numerator * (lcm / a.denominator) < b.numerator * (lcm / b.denominator);
        }

        public static bool operator <(Fraction a, int b)
        {
            return ((float)a.numerator / a.denominator) < b; 
        }
        
        public static bool operator <(Fraction a, float b)
        {
            return ((float)a.numerator / a.denominator) < b; 
        }
        
        // GreaterThan
        public static bool operator >(Fraction a, Fraction b)
        { 
            int lcm = LeastCommonMultiple(a.denominator, b.denominator);
            return a.numerator * (lcm / a.denominator) > b.numerator * (lcm / b.denominator);
        }

        public static bool operator >(Fraction a, int b)
        {
            return ((float)a.numerator / a.denominator) > b; 
        }
        
        public static bool operator >(Fraction a, float b)
        {
            return ((float)a.numerator / a.denominator) > b; 
        }
        
        //LessOrEqual
        public static bool operator <=(Fraction a, Fraction b)
        {
            int lcm = LeastCommonMultiple(a.denominator, b.denominator);
            return a.numerator * (lcm / a.denominator) <= b.numerator * (lcm / b.denominator);
        }
        
        public static bool operator <=(Fraction a, int b)
        {
            return ((float)a.numerator / a.denominator) <= b; 
        }
        
        public static bool operator <=(Fraction a, float b)
        {
            return ((float)a.numerator / a.denominator) <= b; 
        }

        //GreaterOrEqual
        public static bool operator >=(Fraction a, Fraction b)
        { 
            int lcm = LeastCommonMultiple(a.denominator, b.denominator);
            return a.numerator * (lcm / a.denominator) >= b.numerator * (lcm / b.denominator);
        }
        
        public static bool operator >=(Fraction a, int b)
        {
            return ((float)a.numerator / a.denominator) >= b; 
        }
        
        public static bool operator >=(Fraction a, float b)
        {
            return ((float)a.numerator / a.denominator) >= b; 
        }
        
        // Positive
        public static Fraction operator +(Fraction a) => a;
    
        // Negative
        public static Fraction operator -(Fraction a) => new Fraction(-a.numerator, a.denominator);

        // Add
        public static Fraction operator +(Fraction a, Fraction b)
        {
            int lcm = LeastCommonMultiple(a.denominator, b.denominator);

            return new Fraction(
                a.numerator * (lcm / a.denominator) +
                b.numerator * (lcm / b.denominator),
                lcm
            );
        }
    
        // Subtract (Negative Add)
        public static Fraction operator -(Fraction a, Fraction b)
            => a + (-b);
    
        // Multiply
        public static Fraction operator *(Fraction a, Fraction b)
            => new Fraction(a.numerator * b.numerator, a.denominator * b.denominator);

        // Divide
        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b.numerator == 0)
            {
                throw new DivideByZeroException();
            }
            return new Fraction(a.numerator * b.denominator, a.denominator * b.numerator);
        }

        #endregion

        #region Helpers

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
        
        #endregion Helpers

        #region Modifiers

        

        public Fraction LowestTerm()
        {
            int gcd = GreatestCommonDivisor(numerator, denominator);
            return SimplifyBy(gcd);
        }

        public bool CanSimplifyBy(int value)
        {
            return Numerator % value == 0 && Denominator % value == 0; 
        }
        
        public Fraction SimplifyBy(int value)
        {
            if (!CanSimplifyBy(value))
            {
                return this;
            }

            return new Fraction(Numerator / value, Denominator / value);
        }

        public Fraction ExpandBy(int value)
        {
            return new Fraction(Numerator * value, Denominator * value);
        }

        #endregion


    }
}
