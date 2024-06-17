using System.Collections;
using System.Collections.Generic;
using Programming.Fraction_Engine;
using UnityEngine;

[CreateAssetMenu (fileName = "New Fraction", menuName = "Card")]
public class CardInfo : ScriptableObject, IFractionable
{
    public Fraction value;
    public Fraction GetValue()
    {
        return value;
    }
}
