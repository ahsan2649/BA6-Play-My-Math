using System;
using System.Collections.Generic;
using Programming.Fraction_Engine;
using UnityEngine;
using static Programming.Enemy.LevelGeneration.AdditionAndSubtraction;
using static Programming.Enemy.LevelGeneration.Multiplication;

namespace Programming.Enemy
{
    public class LevelGeneration : MonoBehaviour
    {
        // This is the function accessed externally, handle with care

        /// <summary>
        /// Returns a list of Fractions that can be used as the "Monster Deck". 
        /// The generation is based on the weightmaps assigned in the Weights class. Feel free to change their values if you want to change the generation.
        /// </summary>
        /// <param name="numberOfCards"></param>
        /// <param name="debug_return_test_cue_instead">If true, the function will return a fixed test cue for debugging, instead of trying to generate one. Use it if my code fails to generate a monster deck. </param>
        /// <returns></returns>
        public static List<Fraction> generateEnemyCue(int numberOfCards = 10, bool debug_return_test_cue_instead = false)
        {
            // Failsave Debug Option
            if (debug_return_test_cue_instead)
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

            // Generate Deck using my functions
            return generateCardDeckUsingWeights(numberOfCards);
        }



        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // SimplyfiedCombinedFractionsSet 1
        private static List<Fraction> SimplyfiedCombinedFractions = new List<Fraction>
        {
            new Fraction(3,2),
            new Fraction(2,1),
            new Fraction(4,9),
            new Fraction(4,3),
            new Fraction(3,2),
            new Fraction(9,8),
            //Inverts
            new Fraction(2,3),
            new Fraction(1,2),
            new Fraction(9,4),
            new Fraction(3,4),
            new Fraction(2,3),
            new Fraction(8,9),
        };


        // Trash
        // Fraction Assembly Set 4,6,8,9
        //Dictionary<string, List<int>> AVAILABLE_NUMERATORS_AS = new Dictionary<string, List<int>>();

        //private int[] DENOMINATOR_ES_3 = [12, 16, 18]; // 6,8,9 x 2

        // Baking
        // float 1 = Starting value weight, float 2 = multiplier
        //Dictionary<AdditionAndSubtraction.NumeratorPhase, Tuple<float, float>> weightmapAddition = new Dictionary<AdditionAndSubtraction.NumeratorPhase, Tuple<float, float>>
        //    {
        //        {NumeratorPhase.JustPlacing,        Tuple.Create(1f,1f) },
        //        {NumeratorPhase.Two,                Tuple.Create(1f,1f) },
        //        {NumeratorPhase.ThreeAndFive,       Tuple.Create(1f,1f) },
        //        {NumeratorPhase.Seven,              Tuple.Create(1f,1f) },
        //        {NumeratorPhase.DirectAccess,       Tuple.Create(1f,1f) },
        //        {NumeratorPhase.PartlyIndirectAccess, Tuple.Create(1f,1f) },
        //    };

        /// <summary>
        /// The generation Type used: Addition, Expansion, Multiplication
        /// </summary>
        public enum Phase { AdditionAndSubtraction, ExpandAndSimplify, Multiplication, Misc }


        /// <summary>
        /// This is the main function for generating a deck of monster cards.
        /// The generation is based on the weightmaps assigned in the Weights class. Feel free to change their values if you want to change the generation.
        /// </summary>
        /// <param name="numberOfCards"></param>
        /// <returns></returns>
        public static List<Fraction> generateCardDeckUsingWeights(int numberOfCards)
        {
            // Create list of Fractions, this represents the deck that will be returned
            List<Fraction> encounterFraction = new List<Fraction>();

            // for number of cards
            for (int i = 0; i < numberOfCards; i++)
            {
                // Choose one random mode based on weight
                Phase phase = GetRandomItem(Weights.standardAssortmentProperbilityWeightmap);

                switch (phase)
                {
                    // Codeblock Addition and Subtraction
                    case Phase.AdditionAndSubtraction:
                        encounterFraction.Add(AdditionAndSubtraction.generateEncounterFraction(NumeratorPhase.PartlyIndirectAccess, DenominatorPhase.Multiple));
                        break;

                    // Codeblock Expand and Simplify
                    case Phase.ExpandAndSimplify:
                        encounterFraction.Add(ExpandingAndSimplifying.generateEncounterFraction());
                        break;

                    // Codeblock Multiplication
                    case Phase.Multiplication:
                        /// !!!! Only generates goal values in numerator
                        encounterFraction.Add(Multiplication.generateEncounterFraction(false));
                        break;

                    // Codeblock Misc
                    case Phase.Misc:
                        break;

                    // Failsafe
                    default:
                        Debug.LogWarning("Failed to find Phase. This indicates you might be missing a newly added Phase in the Codeblock. Come here and add it. Returned 11/11 Fraction");
                        encounterFraction.Add(new Fraction(11, 11));
                        break;
                }
            }

            return encounterFraction;
        }
    
        /// <summary>
        /// Holds all the informations on propabilities for the different fraction-generation-types. 
        /// The float value inside the dictonaries is the weight; change it to affect the distribution (higher -> more likely)
        /// </summary>
        public static class Weights
        {
            // Standard Assortment

            /// <summary>
            /// Weightmap used to pick between these generation phases for the generated encounter fraction.
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<Phase, float> standardAssortmentProperbilityWeightmap = new Dictionary<Phase, float>
            {
                /// Default Weightmap
                { Phase.AdditionAndSubtraction, 1f },
                { Phase.ExpandAndSimplify, 1f },
                { Phase.Multiplication, 15f },
                { Phase.Misc, 0f }, /// Currently Misc is not implemented
            };

            // Addition and Subtraction

            /// <summary>
            /// Weightmap used to pick between these generation styles when deciding on which numerator to pick for addition and subtraction.
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<NumeratorPhase, float> numeratorPhaseProperbilityWeightmap = new Dictionary<NumeratorPhase, float>
            {
                /// Default Weightmap
                { NumeratorPhase.JustPlacing, 1f },
                { NumeratorPhase.MakeOnes, 1f },
                { NumeratorPhase.Two, 1f },
                { NumeratorPhase.ThreeAndFive, 1f },
                { NumeratorPhase.Seven, 1f },
                { NumeratorPhase.DirectAccess, 1f },
                { NumeratorPhase.PartlyIndirectAccess, 1f },
            };

            /// <summary>
            /// Weightmap used to pick between these generation styles when deciding on which denominator to pick for addition and subtraction.
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<DenominatorPhase, float> denominatorPhaseProperbilityWeightmap = new Dictionary<DenominatorPhase, float>
            {
                /// Default Weightmap
                { DenominatorPhase.Single, 1f },
                { DenominatorPhase.Multiple, 1f },
            };

            /// Expanding/Simplifying

            /// <summary>
            /// Weightmap used to pick between these generation styles when deciding on which base fraction to pick. This fraction will than get expanded by an amount. 
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<ExpandingAndSimplifying.ExpandingBaseFractionPhase, float> expandingFractionBaseProperbilityWeightmap = new Dictionary<ExpandingAndSimplifying.ExpandingBaseFractionPhase, float>
            {
                /// Default Weightmap
                { ExpandingAndSimplifying.ExpandingBaseFractionPhase.PickFromSimplyfiedCombinedFractions, 20f },
                { ExpandingAndSimplifying.ExpandingBaseFractionPhase.PickFromAdditionSubtraction, 1f },
            };

            /// <summary>
            /// Weightmap used to pick between these generation styles when deciding on which base to multiply with for the expansion of the fraction. 
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<ExpandingAndSimplifying.ExpansionPhase, float> expandingPhaseProperbilityWeightmap = new Dictionary<ExpandingAndSimplifying.ExpansionPhase, float>
            {
                /// Default Weightmap
                { ExpandingAndSimplifying.ExpansionPhase.SingleTwo, 1f },
                { ExpandingAndSimplifying.ExpansionPhase.SingleThree, 1f },
                { ExpandingAndSimplifying.ExpansionPhase.Multicomp, 1f },
                { ExpandingAndSimplifying.ExpansionPhase.MulticompOther, 1f },
                { ExpandingAndSimplifying.ExpansionPhase.NoneBase, 1f },
            };

            /// Multiplication

            /// <summary>
            /// Weightmap used to pick between these generation styles when deciding on which first value / goal to use for the numerator/denominator of the goal fraction. 
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<ChallangeSet, float> challangeSetProperbilityWeightmap = new Dictionary<ChallangeSet, float>
            {
                /// Default Weightmap
                { ChallangeSet.Direkt, 10f },
                { ChallangeSet.Indirekt, 1f },
                { ChallangeSet.IndirektSubtract, 10f },
                { ChallangeSet.IndirektSecondDegree, 1f },
            };

            /// <summary>
            /// Weightmap used to pick between these generation styles when deciding on which second value to use for the numerator/denominator of the goal fraction. 
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<SecondarValueGenerationTypes, float> secondaryValueProperbilityWeightmap = new Dictionary<SecondarValueGenerationTypes, float>
            {
                /// Default Weightmap
                { SecondarValueGenerationTypes.X, 0f },         /// p (x) is not implemented so for now its 0
                { SecondarValueGenerationTypes.One, 1f },
                { SecondarValueGenerationTypes.Specific, 0f },  /// The specific value doesnt work perfectly, so for now its 0
            };
        }
    
        /// <summary>
        /// Holds information and functions for the Addition type of fraction-generation. 
        /// The fractions generated using this type, will have a numerator mostly exclusively reachable through adding or subtracting fraction, made from the hand cards, together.
        /// The denominator is simply picked from one of the hand cards. (Which cards are those is specified inside the class)
        /// The enums can be used for the weight dictonaries in the Weight class.
        /// </summary>
        public static class AdditionAndSubtraction
        {
            public enum NumeratorPhase { JustPlacing, MakeOnes, Two, ThreeAndFive, Seven, DirectAccess, PartlyIndirectAccess };
            public enum DenominatorPhase { Single, Multiple };

            // Add Dict here (Only Phase included)
            // Addition and Subtraction
            private static Dictionary<NumeratorPhase, int[]> NumeratorPhaseNumberRef = new Dictionary<NumeratorPhase, int[]>
            {
                { NumeratorPhase.JustPlacing,           new int[] {4,6,8 } },
                { NumeratorPhase.MakeOnes,              new int[] {1 } },
                { NumeratorPhase.Two,                   new int[] {2 } },
                { NumeratorPhase.ThreeAndFive,          new int[] {3,5 } },
                { NumeratorPhase.Seven,                 new int[] {7 } },
                { NumeratorPhase.DirectAccess,          new int[] {10,12,14,16,17,18 } },
                { NumeratorPhase.PartlyIndirectAccess,  new int[] {11,13,15,19,20 } },
            };

            private static Dictionary<DenominatorPhase, int[]> DenominatorPhaseNumberRef = new Dictionary<DenominatorPhase, int[]>
            {
                { DenominatorPhase.Single,              new int[] {9 } },
                { DenominatorPhase.Multiple,            new int[] {4,8,9 } },
            };

            /// <summary>
            /// Returns a fraction generated randomly from the exclusivly given phases.
            /// </summary>
            /// <param name="numeratorPhase"></param>
            /// <param name="denominatorPhase"></param>
            static internal Fraction generateEncounterFraction(NumeratorPhase numeratorPhase, DenominatorPhase denominatorPhase, int[] customNumerators = null, int[] customDenominators = null)
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
                randomIndex = UnityEngine.Random.Range(0, denominatorList.Length);
                denominator = denominatorList[randomIndex];

                // create a fraction from both
                Fraction fraction = new Fraction(numerator, denominator);

                return fraction;
            }
        } // They are public so we can access their enums

        /// <summary>
        /// Holds information and functions for the Expanding & Simplifying type of fraction-generation.
        /// The fractions are generated using either a preset of SimplyfiedCombinedFraction (Inside LevelGeneration class) or using the Addition type. 
        /// It then expands this fraction with a certain number (specified in this class under ExpansionPhaseFactorRef).
        /// </summary>
        public static class ExpandingAndSimplifying
        {
            public enum ExpansionPhase { SingleTwo, SingleThree, Multicomp, MulticompOther, NoneBase }
            public enum ExpandingBaseFractionPhase { PickFromSimplyfiedCombinedFractions, PickFromAdditionSubtraction }

            // Refs for Calculation based on phase
            private static Dictionary<ExpansionPhase, int[]> ExpansionPhaseFactorRef = new Dictionary<ExpansionPhase, int[]>
            {
                { ExpansionPhase.SingleTwo,              new int[] {2 } },
                { ExpansionPhase.SingleThree,            new int[] {3 } },
                { ExpansionPhase.Multicomp,              new int[] {8,9 } },
                { ExpansionPhase.MulticompOther,         new int[] {6,12 } },
                { ExpansionPhase.NoneBase,                new int[] {5,7 } },
            };

            /// <summary>
            /// Generates a Fraction using the Expanding and Simplifying Algorithm and the weigth in Weights.
            /// This function will also access the Addition and Subtraction weights, since the PickFromAdditionSubtraction-Phase first generates Expansion using Addition and Subtraction
            /// </summary>
            /// <returns>Random Fraction</returns
            internal static Fraction generateEncounterFraction()
                // Need AdditionAndSubtraction Phase Info since Expandion and Simplifying is just Addition/Subtraction Fractions expanded/simplified
            {
                ExpandingBaseFractionPhase expandingBaseFractionPhase = GetRandomItem(Weights.expandingFractionBaseProperbilityWeightmap);

                Fraction fraction;

                NumeratorPhase numeratorPhase = GetRandomItem<NumeratorPhase>(Weights.numeratorPhaseProperbilityWeightmap);
                DenominatorPhase denominatorPhase = GetRandomItem<DenominatorPhase>(Weights.denominatorPhaseProperbilityWeightmap);
                ExpansionPhase expansionPhase = GetRandomItem(Weights.expandingPhaseProperbilityWeightmap);


                switch (expandingBaseFractionPhase)
                {
                    // Codeblock for Picking Base Fraction from List of Simplyfied Combined Fractions
                    case ExpandingBaseFractionPhase.PickFromSimplyfiedCombinedFractions:
                        fraction = GetRandomValueFromList(SimplyfiedCombinedFractions);
                        break;
                
                    // Codeblock for Picking Base Fraction from Addition/Subtraction
                    case ExpandingBaseFractionPhase.PickFromAdditionSubtraction:
                        // Since both Single two and three don't include expansions of 4 and 6 in their challanges we have to check for them and instead include a custom number list
                        switch (expansionPhase)
                        {
                            case ExpansionPhase.SingleTwo:
                                fraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators: new int[] { 6, 8, 9 });
                                break;
                            case ExpansionPhase.SingleThree:
                                fraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators: new int[] { 8, 9 });
                                break;
                            default:
                                fraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase);
                                break;
                        }
                        // Expand Fraction with value
                        // Standard random pick, from factors in ref dict
                        int[] expansionFaktorArray = ExpansionPhaseFactorRef[expansionPhase];
                        int randomIndex = UnityEngine.Random.Range(0, expansionFaktorArray.Length);
                        Fraction expandedFraction = fraction.ExpandBy(expansionFaktorArray[randomIndex]);
                        break;
                
                    // Failsave
                    default:
                        Debug.Log("Could not find ExpandingBaseFractionPhase, returned 11/11 Fraction");
                        return new Fraction(11, 11);
                }

                return fraction;
            }
        }

        /// <summary>
        /// Holds information and functions for the Multiplication type of fraction-generation.
        /// The fractions are generated using a predefined collection of through multiplation reachable numbers.
        /// For each number of multiplication steps needed (represented with the number of denominator cards p ) the system generates a denominator accordingly
        /// </summary>
        public static class Multiplication
        {
            const int P_DIREKTS = 2;
            const int P_INDIREKTS = 3;
            const int P_SUBSTRACT_INDIREKTS = 1;
            const int P_INDIREKTS_SECOND_DEGREE = 5;
            const int P_NON_DIREKTS = 4;

            // +++++++++++++++++++++++++++++++++++++++++++

            // This list keeps track of the Base Cards used, changing these will only have an effect on secondary values... for now
            public static List<int> bases = new List<int> { 4, 6, 8, 9 };

            // These are the numbers that can be picked as goals for the different generative modes
            private static List<int> Direkts = new List<int>
            {
                1, // Could be too easy
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

            private static List<int> Indirekts = new List<int>
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

            private static List<int> IndirektsSecondDegree = new List<int>
            {
                27,
                243,
            };

            private static List<int> IndirektsSubstract = new List<int>
            {
                3,
                12,
                18,
                162,
                108,
            };

            // +++++++++++++++++++++++++++++++++++++++++++

            public enum ChallangeSet { Base, Direkt, DirektSubtract, Indirekt, IndirektSubtract, NonDirekt, IndirektSecondDegree }

            public enum SecondarValueGenerationTypes { X, One, Specific }



            // +++++++++++++++++++++++++++++++++++++++++++

            /// <summary>
            /// generates one Encounter Fraction, based on the ChallangeSet-Weightmap. Weightmap can be edited remotely.
            /// </summary>
            /// <param name="make_first_value_the_numerator">Decides how to return the fraction; with the carefully generated value in the numerator or in the denominator. Having this as false will make it harder to solve for the player, but will force them to use multiplication</param>
            /// <returns></returns>
            internal static Fraction generateEncounterFraction(bool make_first_value_the_numerator)
            {
                Fraction fraction;

                int firstValue;
                int p;

                // use a random generation set based on weightmap
                // for the generation, programm picks from the list of possible numbers to choose from
                ChallangeSet set = GetRandomItem<ChallangeSet>(Weights.challangeSetProperbilityWeightmap);
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

                    switch (pickRandomValueByWeight(Weights.secondaryValueProperbilityWeightmap))
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
        } // Public for enums, but also the bases list has to be changed based on which cards are accessible through the hand



        // These functions make calculating in this script a lot easier, they are private and only for me :3

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

        private static T GetRandomValueFromList<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentNullException("The list is empty or null.");
            }

            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }

        private static T GetRandomItem<T>(Dictionary<T, float> weights)
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

        // This function is for debug testing, to see if the generation works
        private void Start()
        {
            List<Fraction> list = generateCardDeckUsingWeights(10);

            //for (int i = 0; i < 10; i++)
            //{
            //    list.Add(Multiplication.createEncounterFraction(true));
            //}

            foreach (var f in list)
            {
                Debug.Log(f.Numerator.ToString() + '/' + f.Denominator.ToString());
            }
        }
    }
}