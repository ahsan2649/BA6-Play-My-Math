using NUnit.Framework;
using Programming.Fraction_Engine;

public class FractionTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void _0_CreateFraction()
    {
        var a = new Fraction(1, 2);
        // Use the Assert class to test conditions
        Assert.AreEqual(a.Numerator, 1);
        Assert.AreEqual(a.Denominator, 2);
    }

    [Test]
    public void _1_EqualFraction()
    {
        var a = new Fraction(1, 3);
        var b = new Fraction(1, 3);
        var c = new Fraction(4, 12);
        
        Assert.AreEqual(a, b);
        Assert.AreEqual(b, c);
        Assert.AreEqual(a, c);
    }

    [Test]
    public void _2_NotEqualFraction()
    {
        var a = new Fraction(5, 6);
        var b = new Fraction(4, 6);
        var c = new Fraction(9, 12);
        
        Assert.AreNotEqual(a,b);
        Assert.AreNotEqual(c,b);
        Assert.AreNotEqual(a,c);
    }

    [Test]
    public void _3_LowestTerm()
    {
        var a = new Fraction(24, 32);
        var b = new Fraction(25, 35);
        var c = new Fraction(64, 72);
        
        Assert.AreEqual(a.LowestTerm(), new Fraction(3, 4));
        Assert.AreEqual(b.LowestTerm(), new Fraction(5, 7));
        Assert.AreEqual(c.LowestTerm(), new Fraction(8, 9));
    }

    [Test]
    public void _4_AddFraction()
    {
        var a = new Fraction(2, 5);
        var b = new Fraction(1, 5);
        var c = new Fraction(1, 10);
        var d = new Fraction(3, 8);
        
        Assert.AreEqual(a+b, new Fraction(3, 5));
        Assert.AreEqual(a+c, new Fraction(1, 2));
        Assert.AreEqual(a+d, new Fraction(31, 40));
        Assert.AreEqual(c+d, new Fraction(19,40));
    }

    [Test]
    public void _5_SubtractFraction()
    {
        var a = new Fraction(7, 10);
        var b = new Fraction(3, 10);
        var c = new Fraction(2, 5);
        var d = new Fraction(4, 6);
        
        Assert.AreEqual(a-b, new Fraction(4, 10));
        Assert.AreEqual(a-c, new Fraction(3, 10));
        Assert.AreEqual(d-a, new Fraction(-1, 30));
        Assert.AreEqual(c-d, new Fraction(-8, 30));
        Assert.AreEqual(c-d, new Fraction(-4, 15));
    }

    [Test]
    public void _6_MultiplyFraction()
    {
        var a = new Fraction(3, 4);
        var b = new Fraction(2, 3);
        var c = new Fraction(6, 7);
        var d = new Fraction(4, 5);
        
        Assert.AreEqual(a*b, new Fraction(6, 12));
        Assert.AreEqual(a*b, new Fraction(1, 2));
        Assert.AreEqual(c*d, new Fraction(24, 35));
    }

    [Test]
    public void _7_DivideFraction()
    {
        var a = new Fraction(3, 8);
        var b = new Fraction(5, 9);
        var c = new Fraction(2, 7);
        
        UnityEngine.Assertions.Assert.AreEqual(a/b, new Fraction(27,40));
        UnityEngine.Assertions.Assert.AreEqual(b/c, new Fraction(35, 18));
        UnityEngine.Assertions.Assert.AreEqual(a/c, new Fraction(21, 16));
        UnityEngine.Assertions.Assert.AreEqual(c/b, new Fraction(18, 35));
    }
    
}
