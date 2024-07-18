using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Programming.Fraction_Engine
{
    public enum ModifyType {
        None, 
        Simplify,
        Expand
    }
    
    public enum OperandType
    {
        None,
        Left,
        LeftModify,
        Right,
        RightModify
    }  
    
    public enum Operation
    {
        Nop,
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    public enum ModifyValue {
        Two = 2,
        Three = 3,
        Five = 5,
        Seven = 7,
    }
    
    [Serializable]
    public class Fraction
    {
        // This is to keep track of the difficulty when fraction is generated in Level Generation. It's used in the Score
        public int difficulty = 0;

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

        public int Wholes => Numerator / Denominator;

        public Fraction(Fraction fraction)
        {
            this.Numerator = fraction.Numerator;
            this.Denominator = fraction.Denominator; 
        }
        
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

        public bool EqualsExact(Fraction other)
        {
            return Numerator == other.Numerator && Denominator == other.Denominator; 
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
                a.LowestTerm().numerator == b.LowestTerm().numerator &&
                a.LowestTerm().denominator == b.LowestTerm().denominator;
        }

        public static bool operator ==(Fraction a, int b)
        {
            return a.Numerator / a.Denominator == b; 
        }

        
        public static bool operator ==(Fraction a, float b)
        {
            return a.Numerator / a.Denominator == b; 
        }
        
        // Inequality
        public static bool operator !=(Fraction a, Fraction b)
        {
            return 
                a.LowestTerm().numerator != b.LowestTerm().numerator ||
                a.LowestTerm().denominator != b.LowestTerm().denominator;
        }

        public static bool operator !=(Fraction a, int b)
        {
            return a.Numerator / a.Denominator != b; 
        }

        public static bool operator !=(Fraction a, float b)
        {
            return a.Numerator / a.Denominator != b; 
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

        public static Fraction CalculateOperation(Fraction leftFraction, Operation operation, Fraction rightFraction)
        {
            if (leftFraction is null || rightFraction is null)
            {
                return null; 
            }

            return operation switch
            {
                Operation.Add => leftFraction.Denominator == rightFraction.Denominator
                    ? leftFraction + rightFraction
                    : null,
                Operation.Subtract => leftFraction.Denominator == rightFraction.Denominator
                    ? leftFraction - rightFraction
                    : null,
                Operation.Multiply => leftFraction * rightFraction,
                Operation.Divide => rightFraction.Numerator != 0 ? leftFraction / rightFraction : null,
                Operation.Nop => null, 
                _ => throw new SwitchExpressionException()
            }; 
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
