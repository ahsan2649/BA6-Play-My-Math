using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Animations;
using static LevelGeneration.AdditionAndSubtraction;

public class LevelGeneration : MonoBehaviour
{
    // Fraction Assembly Set 4,6,8,9
    //Dictionary<string, List<int>> AVAILABLE_NUMERATORS_AS = new Dictionary<string, List<int>>();

    //private int[] DENOMINATOR_ES_3 = [12, 16, 18]; // 6,8,9 x 2

    public class AdditionAndSubtraction
    {
        public enum NumeratorPhase { JustPlacing, MakeOnes, Two, ThreeAndFive, Seven, DirectAccess, PartlyIndirectAccess };
        public enum DenominatorPhase { Single, Multiple };

        // Add Dict here (Only Phase included)
        // Addition and Subtraction
        static Dictionary<NumeratorPhase, int[]> NumeratorPhaseNumberRef = new Dictionary<NumeratorPhase, int[]> 
        {
            { NumeratorPhase.JustPlacing,           new int[] {4,6,8 } },
            { NumeratorPhase.MakeOnes,              new int[] {1 } },
            { NumeratorPhase.Two,                   new int[] {2 } },
            { NumeratorPhase.ThreeAndFive,          new int[] {3,5 } },
            { NumeratorPhase.Seven,                 new int[] {7 } },
            { NumeratorPhase.DirectAccess,          new int[] {10,12,14,16,17,18 } },
            { NumeratorPhase.PartlyIndirectAccess,  new int[] {11,13,15,19,20 } },
        };
        
        static Dictionary<DenominatorPhase, int[]> DenominatorPhaseNumberRef = new Dictionary<DenominatorPhase, int[]>
        {
            { DenominatorPhase.Single,              new int[] {9 } },
            { DenominatorPhase.Multiple,            new int[] {4,8,9 } },
        };

        /// <summary>
        /// Returns a fraction generated randomly from the exclusivly given phases.
        /// </summary>
        /// <param name="numeratorPhase"></param>
        /// <param name="denominatorPhase"></param>
        static public Fraction generateEncounterFraction(NumeratorPhase numeratorPhase, DenominatorPhase denominatorPhase, int[] customNumerators = null, int[] customDenominators = null)
        {
            int numerator ;
            // Take one numerator from Dict
            int[] numeratorList;
            // If there are custom denomnumerators set pick those instead
            if (customNumerators != null)
                numeratorList = customNumerators;
            else
                numeratorList = NumeratorPhaseNumberRef[numeratorPhase]; 
            
            int randomIndex;
            // Adds 9 (Denominator number) to the list of numerators in multiple denominators Phase
            if (denominatorPhase == DenominatorPhase.Multiple && customNumerators != null) {
                // Adds a chance for a 9 into the denominator calculation
                randomIndex = Random.Range(0, numeratorList.Length+1);
                if (randomIndex == numeratorList.Length + 1)
                {
                    numerator = 9;
                }
                else
                {
                    numerator = numeratorList[randomIndex];
                }
            }
            else
            {
                // Standard random pick
                randomIndex = Random.Range(0, numeratorList.Length);
                numerator = numeratorList[randomIndex];
            }

            int denominator;
            // take one denominator from Dict
            int[] denominatorList;
            // If there are custom denominators set pick those instead
            if (customDenominators != null)
                denominatorList = customDenominators;
            else
                denominatorList = DenominatorPhaseNumberRef[denominatorPhase];

            // Standard random pick
            randomIndex = Random.Range(0, numeratorList.Length);
            denominator = denominatorList[randomIndex];

            // create a fraction from both
            Fraction fraction = new Fraction(numerator, denominator);
            
            return fraction;
        }
    }

    /*
     * 
     */
    public class ExpandingAndSimplifying
    {
        public enum ExpansionPhase { SingleTwo, SingleThree, Multicomp, MulticompOther}

        // Currently the Simplify Phase is not included

        // Refs for Calculation based on phase
        static Dictionary<ExpansionPhase, int[]> ExpansionPhaseFactorRef = new Dictionary<ExpansionPhase, int[]>
        {
            { ExpansionPhase.SingleTwo,              new int[] {2 } },
            { ExpansionPhase.SingleThree,            new int[] {3 } },
            { ExpansionPhase.Multicomp,              new int[] {8,9 } },
            { ExpansionPhase.MulticompOther,         new int[] {5,7 } },
        };

        /// <summary>
        /// Generates a Fraction based on the phase parameters
        /// </summary>
        /// <param name="numeratorPhase"></param>
        /// <param name="denominatorPhase"></param>
        /// <param name="expansionPhase"></param>
        /// <returns>Random Fraction</returns
        public static Fraction generateEncounterFraction(AdditionAndSubtraction.NumeratorPhase numeratorPhase, AdditionAndSubtraction.DenominatorPhase denominatorPhase, ExpansionPhase expansionPhase)
        // Need AdditionAndSubtraction Phase Info since Expandion and Simplifying is just Addition/Subtraction Fractions expanded/simplified
        {
            Fraction additionFraction;
            // Since both Single two and three don't include expansions of 4 and 6 in their challan´ges we have to check for them and instead include a custom number list
            switch (expansionPhase)
            {
                case ExpansionPhase.SingleTwo:
                    additionFraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators : new int[] {6, 8, 9 });
                    break;
                case ExpansionPhase.SingleThree:
                    additionFraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators: new int[] {8, 9 });
                    break;
                default:
                    additionFraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase);
                    break;
            }

            // Expand Fraction with value
            // Standard random pick, from factors in ref dict
            int[] expansionFaktorArray = ExpansionPhaseFactorRef[expansionPhase];
            int randomIndex = Random.Range(0, expansionFaktorArray.Length);
            Fraction expandedFraction = additionFraction.ExpandBy(expansionFaktorArray[randomIndex]);

            return expandedFraction;
        }
    }

    public enum Phase { AdditionAndSubtraction, ExpandAndSimplify}
    public static List<Fraction> generateCardDeckUsingWeights(int numberOfCards, int additionWeight=0, int expandingWeight=0)
    {
        // Create list of Fractions, this represents the deck that will be returned
        List<Fraction> encounterFraction = new List<Fraction>();

        // for number of cards
        for (int i = 0; i < numberOfCards; i++)
        {
            // Choose one random Mode based on weight
            int totalWeight = additionWeight + expandingWeight;
            int randomNumber = Random.Range(0, totalWeight);
            if (randomNumber <= additionWeight)
            {
                // !!!! Cards only generate most difficult Phases
                encounterFraction.Add(AdditionAndSubtraction.generateEncounterFraction(NumeratorPhase.PartlyIndirectAccess, DenominatorPhase.Multiple));
            }
            else
            {
                // !!!! Cards only generate most difficult Phases
                encounterFraction.Add(ExpandingAndSimplifying.generateEncounterFraction(NumeratorPhase.PartlyIndirectAccess, DenominatorPhase.Multiple, ExpandingAndSimplifying.ExpansionPhase.MulticompOther));
            }
        }

        return encounterFraction;
    }

    public static List<Fraction> generateEnemyCue()
    {
        return new List<Fraction>();
    }
}
