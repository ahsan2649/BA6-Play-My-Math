using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;
using static LevelGeneration.AdditionAndSubtraction;
using static LevelGeneration.Multiplication;

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
            int numerator;
            // Take one numerator from Dict
            int[] numeratorList;
            // If there are custom denomnumerators set pick those instead
            if (customNumerators != null)
                numeratorList = customNumerators;
            else
                numeratorList = NumeratorPhaseNumberRef[numeratorPhase];

            int randomIndex;
            // Adds 9 (Denominator number) to the list of numerators in multiple denominators Phase
            if (denominatorPhase == DenominatorPhase.Multiple && customNumerators != null)
            {
                // Adds a chance for a 9 into the denominator calculation
                randomIndex = UnityEngine.Random.Range(0, numeratorList.Length + 1);
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
                randomIndex = UnityEngine.Random.Range(0, numeratorList.Length);
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
            randomIndex = UnityEngine.Random.Range(0, numeratorList.Length);
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
        public enum ExpansionPhase { SingleTwo, SingleThree, Multicomp, MulticompOther }

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
                    additionFraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators: new int[] { 6, 8, 9 });
                    break;
                case ExpansionPhase.SingleThree:
                    additionFraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators: new int[] { 8, 9 });
                    break;
                default:
                    additionFraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase);
                    break;
            }

            // Expand Fraction with value
            // Standard random pick, from factors in ref dict
            int[] expansionFaktorArray = ExpansionPhaseFactorRef[expansionPhase];
            int randomIndex = UnityEngine.Random.Range(0, expansionFaktorArray.Length);
            Fraction expandedFraction = additionFraction.ExpandBy(expansionFaktorArray[randomIndex]);

            return expandedFraction;
        }
    }

    public enum Phase { AdditionAndSubtraction, ExpandAndSimplify }
    public static List<Fraction> generateCardDeckUsingWeights(int numberOfCards, int additionWeight = 0, int expandingWeight = 0)
    {
        // Create list of Fractions, this represents the deck that will be returned
        List<Fraction> encounterFraction = new List<Fraction>();

        // for number of cards
        for (int i = 0; i < numberOfCards; i++)
        {
            // Choose one random Mode based on weight
            int totalWeight = additionWeight + expandingWeight;
            int randomNumber = UnityEngine.Random.Range(0, totalWeight);
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

    // Baking
    // float 1 = Starting value weight, float 2 = multiplier
    Dictionary<AdditionAndSubtraction.NumeratorPhase, Tuple<float, float>> weightmapAddition = new Dictionary<AdditionAndSubtraction.NumeratorPhase, Tuple<float, float>>
        {
            {NumeratorPhase.JustPlacing,        Tuple.Create(1f,1f) },
            {NumeratorPhase.Two,                Tuple.Create(1f,1f) },
            {NumeratorPhase.ThreeAndFive,       Tuple.Create(1f,1f) },
            {NumeratorPhase.Seven,              Tuple.Create(1f,1f) },
            {NumeratorPhase.DirectAccess,       Tuple.Create(1f,1f) },
            {NumeratorPhase.PartlyIndirectAccess, Tuple.Create(1f,1f) },
        };

    public static List<Fraction> generateEnemyCue()
    {
        List<Fraction> testCue = new List<Fraction>
        {
            new Fraction(6,9),
            new Fraction(8,9),
            new Fraction(4,9),
            new Fraction(2,9),
        };

        return testCue;
    }

    private static void pickFromStandardAssortment()
    {
        Dictionary<Phase, float> baseWeightmap = new Dictionary<Phase, float>();
        Dictionary<Phase, float> currentWeightmap = new Dictionary<Phase, float>(baseWeightmap); //Duplicates the baseWeightmap

        /// weight calculation percantage

        int time = 10;
        float baseWeigth = 10f;
        float baseMultiplicator = 1f;

        float thisWeight = baseWeigth + time * baseMultiplicator;

        // Generate card with currentWeight
        // Add weights together

        // add time to weight

        // go through each weight an apply multiplier
        float oldweight = 1f;
        float newweight = oldweight + baseMultiplicator;
    }

    private static void calculateWeigths()
    {

    }

    public class Multiplication
    {
        const int P_DIREKTS = 2;
        const int P_INDIREKTS = 3;
        const int P_SUBSTRACT_INDIREKTS = 1;
        const int P_INDIREKTS_SECOND_DEGREE = 5;
        const int P_NON_DIREKTS = 4;

        // +++++++++++++++++++++++++++++++++++++++++++

        // This list keeps track of the Base Cards used, changing these will only have an effect on secondary values... for now
        static List<int> bases = new List<int> { 4, 6, 8, 9 };

        // These are the numbers that can be picked as goals for the different generative modes
        public static List<int> Direkts = new List<int>
        {
            1,
            16,
            32,
            64,
            24,
            48,
            72,
            36,
            54,
            81,
        };

        public static List<int> Indirekts = new List<int>
        {
            128,
            256,
            512,
            96,
            192,
            384,
            144,
            288,
            579,
            216,
            432,
            324,
            648,
            486,
            729,
        };

        public static List<int> IndirektsSecondDegree = new List<int>
        {
            27,
            243,
        };

        public static List<int> IndirektsSubstract = new List<int>
        {
            3,
            12,
            18,
            162,
            108,
        };

        // +++++++++++++++++++++++++++++++++++++++++++

        public enum ChallangeSet { Base, Direkt, DirektSubtract, Indirekt, IndirektSubtract, NonDirekt, IndirektSecondDegree }
        
        /// <summary>
        /// Weightmap used to pick between these generation styles when deciding on which first value / goal to use for the numerator/denominator of the goal fraction. 
        /// Feel free to access and change the values.
        /// </summary>
        public static Dictionary<ChallangeSet, float> challangeSetProperbilityWeightmap = new Dictionary<ChallangeSet, float>
        {
            /// Default Weightmap
            { ChallangeSet.Direkt, 1f },
            { ChallangeSet.Indirekt, 1f },
            { ChallangeSet.IndirektSubtract, 1f },
            { ChallangeSet.IndirektSecondDegree, 1f },
        };

        public enum SecondarValueGenerationTypes { X, One, Specific }

        /// <summary>
        /// Weightmap used to pick between these generation styles when deciding on which second value to use for the numerator/denominator of the goal fraction. 
        /// Feel free to access and change the values.
        /// </summary>
        public static Dictionary<SecondarValueGenerationTypes, float> secondaryValueProperbilityWeightmap = new Dictionary<SecondarValueGenerationTypes, float>
        {
            /// Default Weightmap
            { SecondarValueGenerationTypes.X, 0f },
            { SecondarValueGenerationTypes.One, 1f },
            /// The specific value doesnt work perfectly, so for now its 0
            { SecondarValueGenerationTypes.Specific, 0f },
        };

        // +++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// generates one Encounter Fraction, based on the ChallangeSet-Weightmap. Weightmap can be edited remotely.
        /// </summary>
        /// <param name="make_first_value_the_numerator">Decides how to return the fraction; with the carefully generated value in the numerator or in the denominator. Having this as false will make it harder to solve for the player, but will force them to use multiplication</param>
        /// <returns></returns>
        public static Fraction createEncounterFraction(bool make_first_value_the_numerator)
        {
            Fraction fraction;

            int firstValue;
            int p;

            // use a random generation set based on weightmap
            // for the generation, programm picks from the list of possible numbers to choose from
            ChallangeSet set = GetRandomItem(challangeSetProperbilityWeightmap);
            switch (set)
            {
                case ChallangeSet.Direkt:
                    firstValue = GetRandomValueFromList(Direkts);
                    p = P_DIREKTS;
                    break;
                case ChallangeSet.Indirekt:
                    firstValue = GetRandomValueFromList(Indirekts);
                    p = P_INDIREKTS;
                    break;
                case ChallangeSet.IndirektSecondDegree:
                    firstValue = GetRandomValueFromList(IndirektsSecondDegree);
                    p = P_INDIREKTS_SECOND_DEGREE;
                    break;
                case ChallangeSet.IndirektSubtract:
                    firstValue = GetRandomValueFromList(Indirekts);
                    p = P_SUBSTRACT_INDIREKTS;
                    break;
                default:
                    Debug.LogWarning("Couldn't find the Challange-Set in this context. You're likely missing the newly added Challange set in this code block. Returned 11 instead");
                    firstValue = 11;
                    p = 1;
                    break;
            }

            int secondValue = pickSecondaryValue(p);

            // creates the fraction either with the goal value being the numerator or denominator, based on bool given
            if (make_first_value_the_numerator)
            {
                fraction = new Fraction(firstValue, secondValue);
            }
            else 
            { 
                fraction = new Fraction(secondValue, firstValue);
            }

            return fraction;

            /// <summary>
            /// Picks a random secondary value based on the weightmap in LevelGeneration and returns it. 
            /// Returns 0, if value should be x (any value works). If you use this function, make sure to cover the 0
            /// </summary>
            /// <param name="p"></param>
            int pickSecondaryValue(int p)
            {
                SecondarValueGenerationTypes pickRandomValueByWeight(Dictionary<SecondarValueGenerationTypes, float> weightmap)
                {
                    float totalWeight = 0f;
                    foreach (KeyValuePair<SecondarValueGenerationTypes, float> pair in weightmap)
                    {
                        totalWeight += pair.Value;
                    }

                    float randomPoint = UnityEngine.Random.Range(0f, totalWeight);

                    foreach (KeyValuePair<SecondarValueGenerationTypes, float> pair in weightmap)
                    {
                        if (randomPoint < pair.Value)
                        {
                            return pair.Key;
                        }
                        randomPoint -= pair.Value;
                    }

                    return default; // Should theoretically never be reached
                }

                switch (pickRandomValueByWeight(secondaryValueProperbilityWeightmap))
                {
                    case (SecondarValueGenerationTypes.X):
                        return 0;
                    case (SecondarValueGenerationTypes.One):
                        return 1;
                    case (SecondarValueGenerationTypes.Specific):
                        // uses p to generate a secondary value that can be reached using p number of bases
                        int x = 1;
                        for (int i = 0; i < p; i++)
                        {
                            x *= GetRandomValueFromList(bases);
                        }
                        return x;
                    default:
                        Debug.LogWarning("Secondary value could not be found. An error in the weightmap or a new ValueGenerationType is likely. Defaulted to 11");
                        return 11;
                }
            }
        }
    }


    public static T GetRandomValueFromList<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new System.ArgumentNullException("The list is empty or null.");
        }

        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }
    public static T GetRandomItem<T>(Dictionary<T, float> weights)
    {
        if (weights == null || weights.Count == 0)
        {
            throw new InvalidOperationException("Weightmap is empty or null.");
        }

        // Calculate the total weight sum.
        var totalWeight = 0f;
        foreach (var weight in weights.Values)
        {
            totalWeight += weight;
        }

        // Generate a random point within this total weight.
        var randomPoint = UnityEngine.Random.value * totalWeight;

        // Iterate over the weightmap to find where this random point would fall.
        foreach (var pair in weights)
        {
            if (randomPoint < pair.Value) // Found the chosen item.
            {
                return pair.Key;
            }
            randomPoint -= pair.Value; // Decrease randomPoint and move to the next item.
        }

        // The code should not reach this point because a return should have happened within the loop.
        throw new InvalidOperationException("Control should not reach this point; check weight calculations.");
    }

    private void Start()
    {
        List<Fraction> list = new List<Fraction>();

        for (int i = 0; i < 10; i++)
        {
            list.Add(Multiplication.createEncounterFraction(true));
        }

        foreach (var f in list)
        {
            Debug.Log(f.Numerator.ToString() + '/' + f.Denominator.ToString());
        }
    }
}