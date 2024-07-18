using System;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using static Programming.Enemy.LevelGeneration.AdditionAndSubtraction;
using static Programming.Enemy.LevelGeneration.Multiplication;

namespace Programming.Enemy
{
    public class LevelGeneration : MonoBehaviour
    {
        #region PublicFunctions
        // This is the function accessed externally, handle with care

        public static GameMode gameMode = GameMode.easy23;

        /// <summary>
        /// Returns a list of Fractions that can be used as the "Monster Deck". 
        /// The generation is based on the weightmaps assigned in the Weights class. Feel free to change their values if you want to change the generation.
        /// </summary>
        /// <param name="numberOfCards"></param>
        /// <param name="debug_return_test_cue_instead">If true, the function will return a fixed test cue for debugging, instead of trying to generate one. Use it if my code fails to generate a monster deck. </param>
        /// <returns></returns>
        public static List<Fraction> generateEnemyCue(bool debug_return_test_cue_instead = false)
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

            // Adds to difficulty so every next generation is harder
            currentDifficulty += difficultyAddedEachRound;

            List<Fraction> enemyCue = generateCardDeckUsingDifficulty(currentDifficulty);

            // Generate Deck using my functions
            return enemyCue;
        }

        static List<Fraction> rewardList23 = new List<Fraction>()
        {
            new Fraction(1,1),
            new Fraction(2,1),
            new Fraction(3,1),
            new Fraction(6,1),
            new Fraction(8,1),
            new Fraction(9,1),
            new Fraction(12,1),
            new Fraction(16,1),
            new Fraction(18,1),
        };

        static List<Fraction> rewardList235 = new List<Fraction>()
        {
            new Fraction(5,1),
        };

        static List<Fraction> rewardList2357 = new List<Fraction>()
        {
            new Fraction(7,1),
        };

        public enum GameMode { easy23, medium235, hard2357 }
        public static Fraction GenerateReward(GameMode gameMode)
        {
            //TODO @Vin: GenerateReward
            Fraction reward = new Fraction(1,1);
            
            switch (gameMode)
            {
                case (GameMode.easy23):
                    reward = GetRandomValueFromList(rewardList23);
                    break;
                case (GameMode.medium235):
                    reward = GetRandomValueFromList(rewardList23);
                    break;
                case (GameMode.hard2357):
                    reward = GetRandomValueFromList(rewardList23);
                    break;
            }

            return reward; 
        }

        /// <summary>
        /// This function calculates the reward tupel based on the current difficulty. 
        /// Call this function BEFORE generating the next EnemyCue with generateEnemyCue.
        /// The tupel is formated like this: Tupel( cardLeftForOneReward, cardsLeftForTwoRewards, CardsLeftForThreeRewards )
        /// </summary>
        /// <returns></returns>
        public static List<int> generateRewardThresholdValues()
        {
            //List<int> rewardTupel = new List<int> { ((int)(currentDifficulty * 0.1f)), ((int)(currentDifficulty * 0.5f)), currentDifficulty };
            List<int> rewardTupel = new List<int> { 2, 4, 6 };

            return rewardTupel;
        }

        // TODO @Vin: Make these dictionaries/lists/...
        // GameMode -> Denominator (also in main menu)
        // List<Difficulty> DifficultyPerEncounter
        // Difficulty -> Type of Generation & Amount

        private static int startDifficulty = 6;
        private static int difficultyAddedEachRound = 3;

        public static int currentDifficulty = startDifficulty - difficultyAddedEachRound;

        

        /// <summary>
        /// Please call this function whenever the game is started from the beginning. 
        /// This will reset the difficulty and make sure that there is an increasing difficulty for each level
        /// </summary>
        public static void resetDifficulty()
        {
            currentDifficulty = startDifficulty - difficultyAddedEachRound;
        }

        internal enum GM
        {
            FB,     // B + p = 2
            FS,    // B + p = 2
            Ads,    // B + p = 4
            Ms,     // B + p = 4
            M,      // B + p = 4
            M2,     // B + p = 4
            M3,     // B + p = 4
        }

        private static Dictionary<int, List<List<GM>>> difficultyToGeneration = new Dictionary<int, List<List<GM>>>
        {
            {6, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB }, //4
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.FB, GM.FB }, //6
                }
            },
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.FS, GM.FB }, //6
                }
            },
            // Adding Ads
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.FS, GM.FS } //8
                }
            },
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.FB, GM.FS, GM.FB, GM.FS }, //10
                }
            },
            {21, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.Ads, GM.FB, GM.FS }, //12
                }
            },
            // Adding Ms
            {24, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ms, GM.FB, GM.FB, GM.FS, GM.FS, GM.FS }, // 14
                }
            },
            {27, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.Ms, GM.FB, GM.FS, GM.FB, GM.FS }, // 14
                }
            },
            // Adding M
            {30, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.Ms, GM.Ads, GM.FB, GM.FS, GM.FS }, // 18
                }
            },
            {33, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.FB, GM.M, GM.Ads, GM.M, GM.FS, GM.FS }, // 22
                }
            },
            {36, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.M2, GM.Ads, GM.M2, GM.FB, GM.M3, GM.FS, GM.M, GM.FB }, // 30
                }
            },
            {39, new List<List<GM>>()
                {
                    new List<GM>() { GM.M3, GM.Ads, GM.M2, GM.Ads, GM.M2, GM.FB, GM.M3, GM.FS, GM.M, GM.FB }, // 34
                }
            },
        };

        private static int maxKeyOfGM = 39;

        static List<GM> getGenerationFromDifficulty(int difficulty)
        {
            // Key is predefined
            if (difficultyToGeneration.ContainsKey(difficulty))
            {
                return GetRandomValueFromList(difficultyToGeneration[difficulty]);
            }
            else if (difficulty > maxKeyOfGM)
            {
                int difference = difficulty - maxKeyOfGM;
                int rounds = difference % 4;
                // Make generation list of maximum
                List<GM> gMList = GetRandomValueFromList(difficultyToGeneration[maxKeyOfGM]);
                // Add filler enemy fractions based on how much difficulty
                for (int i = 0; i < rounds; i++) 
                {
                    gMList.Add(GM.M);
                }
                return gMList;
            }
            Debug.LogWarning("Difficulty was not in difficultyToGeneration, returned empty list instead");
            return new List<GM>();
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // BaseCombinedFraction Set
        private static List<Fraction> BaseCombinedFractions23 = new List<Fraction>
        {
            new Fraction(4,4),
            new Fraction(4,6),
            new Fraction(4,8),
            new Fraction(4,9),
            new Fraction(6,6),
            new Fraction(6,8),
            new Fraction(6,9),
            new Fraction(8,8),
            new Fraction(8,9),
            new Fraction(9,9),
        };

        private static List<Fraction> BaseCombinedFractions235 = new List<Fraction>
        {
            new Fraction(4,4),
            new Fraction(4,6),
            new Fraction(4,8),
            new Fraction(4,9),
            new Fraction(6,6),
            new Fraction(6,8),
            new Fraction(6,9),
            new Fraction(8,8),
            new Fraction(8,9),
            new Fraction(9,9),
            new Fraction(4,5),
            new Fraction(5,5),
            new Fraction(5,6),
            new Fraction(5,8),
            new Fraction(5,9),
            new Fraction(9,10),
        };

        private static List<Fraction> BaseCombinedFractions2357 = new List<Fraction>
        {
            new Fraction(4,4),
            new Fraction(4,6),
            new Fraction(4,8),
            new Fraction(4,9),
            new Fraction(6,6),
            new Fraction(6,8),
            new Fraction(6,9),
            new Fraction(8,8),
            new Fraction(8,9),
            new Fraction(9,9),
            new Fraction(4,5),
            new Fraction(5,5),
            new Fraction(5,6),
            new Fraction(5,8),
            new Fraction(5,9),
            new Fraction(9,10),
            new Fraction(4,7),
            new Fraction(6,7),
            new Fraction(7,8),
            new Fraction(7,9),
        };

        static Fraction getBaseFraction()
        {
            Fraction fraction = new Fraction(11,1);

            switch (gameMode)
            {
                case GameMode.easy23:
                    fraction = GetRandomValueFromList(BaseCombinedFractions23);
                    break;
                case GameMode.medium235:
                    fraction = GetRandomValueFromList(BaseCombinedFractions235);
                    break;
                case GameMode.hard2357:
                    fraction = GetRandomValueFromList(BaseCombinedFractions2357);
                    break;
            }
            return fraction;
        }

        // SimplyfiedCombinedFractionsSet 1
        private static List<Fraction> SimplyfiedCombinedFractions23 = new List<Fraction>
        {
            new Fraction(1,3),
            new Fraction(1,2),
            new Fraction(3,4),
            new Fraction(2,3),
            // Inverts
            new Fraction(2,1),
            new Fraction(4,3),
            new Fraction(3,2),
        };

        private static List<Fraction> SimplyfiedCombinedFractions235 = new List<Fraction>
        {
            new Fraction(1,3),
            new Fraction(1,2),
            new Fraction(3,4),
            new Fraction(2,3),
            new Fraction(2,5),
            new Fraction(3,5),
            new Fraction(4,5),
            // Inverts
            new Fraction(2,1),
            new Fraction(4,3),
            new Fraction(3,2),
            new Fraction(5,3),
            new Fraction(5,4),
        };

        private static List<Fraction> SimplyfiedCombinedFractions2357 = new List<Fraction>
        {
            new Fraction(1,3),
            new Fraction(1,2),
            new Fraction(3,4),
            new Fraction(2,3),
            new Fraction(2,5),
            new Fraction(3,5),
            new Fraction(4,5),
            new Fraction(2,7),
            new Fraction(3,7),
            new Fraction(4,7),
            new Fraction(5,7),
            // Inverts
            new Fraction(2,1),
            new Fraction(4,3),
            new Fraction(3,2),
            new Fraction(5,3),
            new Fraction(5,4),
            new Fraction(7,4),
            new Fraction(7,5),
        };

        static Fraction getSimplifiedCombinedFraction()
        {
            Fraction fraction = new Fraction(11, 1);

            switch (gameMode)
            {
                case GameMode.easy23:
                    fraction = GetRandomValueFromList(SimplyfiedCombinedFractions23);
                    break;
                case GameMode.medium235:
                    fraction = GetRandomValueFromList(SimplyfiedCombinedFractions235);
                    break;
                case GameMode.hard2357:
                    fraction = GetRandomValueFromList(SimplyfiedCombinedFractions2357);
                    break;
            }
            return fraction;
        }

        //private static List<Fraction> SimplyfiedCombinedFractionIndirect = new List<Fraction>
        //{
        //    new Fraction(3,8),
        //    new Fraction(1,3),
        //    new Fraction(2,1),
        //    new Fraction(1,9),
        //    new Fraction(1,4),
        //    new Fraction(2,9),
        //    new Fraction(1,6),
        //    new Fraction(1,8),
        //};

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
        public static List<Fraction> generateCardDeckUsingDifficulty(int difficulty)
        {
            // Create list of Fractions, this represents the deck that will be returned
            List<Fraction> encounterFractions = new List<Fraction>();

            List<GM> generationList = getGenerationFromDifficulty(difficulty);

            // for number of cards
            foreach (GM generation in generationList)
            {
                {
                    Fraction encounterFraction;
                    switch (generation)
                    {
                        // Addition Directs
                        case GM.Ads:
                            encounterFraction = AdditionAndSubtraction.generateEncounterFraction();
                            encounterFraction.difficulty = 40;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Fraction Bases
                        case GM.FB:
                            encounterFraction = getBaseFraction();
                            encounterFraction.difficulty = 15;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Fraction Simplified Combines
                        case GM.FS:
                            encounterFraction = getSimplifiedCombinedFraction();
                            encounterFraction.difficulty = 20;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Directs
                        case GM.Ms:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = Multiplication.generateEncounterFraction(ChallangeSet.Simple);
                            encounterFraction.difficulty = 55;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Indirects
                        case GM.M:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = Multiplication.generateEncounterFraction(ChallangeSet.Normal);
                            encounterFraction.difficulty = 80;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Indirects
                        case GM.M2:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = Multiplication.generateEncounterFraction(ChallangeSet.TwoComposites);
                            encounterFraction.difficulty = 80;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Indirects
                        case GM.M3:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = Multiplication.generateEncounterFraction(ChallangeSet.TwoComposites);
                            encounterFraction.difficulty = 80;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Failsafe
                        default:
                            Debug.LogWarning("Failed to find Phase. This indicates you might be missing a newly added Phase in the Codeblock. Come here and add it. Returned 11/11 Fraction");
                            encounterFractions.Add(new Fraction(11, 11));
                            break;
                    }
                }
            }

            return encounterFractions;
        }

        ///// <summary>
        ///// Holds all the informations on propabilities for the different fraction-generation-types. 
        ///// The float value inside the dictonaries is the weight; change it to affect the distribution (higher -> more likely)
        ///// </summary>
        public static class Weights
        {
            // Standard Assortment

            /// <summary>
            /// Weightmap used to pick between these generation phases for the generated encounter fraction.
            /// Feel free to access and change the values.
            /// </summary>
            //public static Dictionary<Phase, float> standardAssortmentProperbilityWeightmap = new Dictionary<Phase, float>
            //{
            //    /// Default Weightmap
            //    { Phase.AdditionAndSubtraction, 1f },
            //    { Phase.ExpandAndSimplify, 1f },
            //    { Phase.Multiplication, 15f },
            //    { Phase.Misc, 0f }, /// Currently Misc is not implemented
            //};

            //// Addition and Subtraction

            /////// <summary>
            /////// Weightmap used to pick between these generation styles when deciding on which numerator to pick for addition and subtraction.
            /////// Feel free to access and change the values.
            /////// </summary>
            ////public static Dictionary<NumeratorPhase, float> numeratorPhaseProperbilityWeightmap = new Dictionary<NumeratorPhase, float>
            ////{
            ////    /// Default Weightmap
            ////    { NumeratorPhase.JustPlacing, 1f },
            ////    { NumeratorPhase.MakeOnes, 1f },
            ////    { NumeratorPhase.Two, 1f },
            ////    { NumeratorPhase.ThreeAndFive, 1f },
            ////    { NumeratorPhase.Seven, 1f },
            ////    { NumeratorPhase.DirectAccess, 1f },
            ////    { NumeratorPhase.PartlyIndirectAccess, 1f },
            ////};

            ///// <summary>
            ///// Weightmap used to pick between these generation styles when deciding on which denominator to pick for addition and subtraction.
            ///// Feel free to access and change the values.
            ///// </summary>
            //public static Dictionary<DenominatorPhase, float> denominatorPhaseProperbilityWeightmap = new Dictionary<DenominatorPhase, float>
            //{
            //    /// Default Weightmap
            //    { DenominatorPhase.Single, 1f },
            //    { DenominatorPhase.Multiple, 1f },
            //};

            ///// Expanding/Simplifying

            ///// <summary>
            ///// Weightmap used to pick between these generation styles when deciding on which base fraction to pick. This fraction will than get expanded by an amount. 
            ///// Feel free to access and change the values.
            ///// </summary>
            //public static Dictionary<ExpandingAndSimplifying.ExpandingBaseFractionPhase, float> expandingFractionBaseProperbilityWeightmap = new Dictionary<ExpandingAndSimplifying.ExpandingBaseFractionPhase, float>
            //{
            //    /// Default Weightmap
            //    { ExpandingAndSimplifying.ExpandingBaseFractionPhase.PickFromSimplyfiedCombinedFractions, 20f },
            //    { ExpandingAndSimplifying.ExpandingBaseFractionPhase.PickFromAdditionSubtraction, 1f },
            //};

            ///// <summary>
            ///// Weightmap used to pick between these generation styles when deciding on which base to multiply with for the expansion of the fraction. 
            ///// Feel free to access and change the values.
            ///// </summary>
            //public static Dictionary<ExpandingAndSimplifying.ExpansionPhase, float> expandingPhaseProperbilityWeightmap = new Dictionary<ExpandingAndSimplifying.ExpansionPhase, float>
            //{
            //    /// Default Weightmap
            //    { ExpandingAndSimplifying.ExpansionPhase.SingleTwo, 1f },
            //    { ExpandingAndSimplifying.ExpansionPhase.SingleThree, 1f },
            //    { ExpandingAndSimplifying.ExpansionPhase.Multicomp, 1f },
            //    { ExpandingAndSimplifying.ExpansionPhase.MulticompOther, 1f },
            //    { ExpandingAndSimplifying.ExpansionPhase.NoneBase, 1f },
            //};

            ///// Multiplication

            ///// <summary>
            ///// Weightmap used to pick between these generation styles when deciding on which first value / goal to use for the numerator/denominator of the goal fraction. 
            ///// Feel free to access and change the values.
            ///// </summary>
            //public static Dictionary<ChallangeSet, float> challangeSetProperbilityWeightmap = new Dictionary<ChallangeSet, float>
            //{
            //    /// Default Weightmap
            //    { ChallangeSet.Direkt, 10f },
            //    { ChallangeSet.Indirekt, 1f },
            //    { ChallangeSet.IndirektSubtract, 10f },
            //    { ChallangeSet.IndirektSecondDegree, 1f },
            //};

            /// <summary>
            /// Weightmap used to pick between these generation styles when deciding on which second value to use for the numerator/denominator of the goal fraction. 
            /// Feel free to access and change the values.
            /// </summary>
            public static Dictionary<SecondarValueGenerationTypes, float> secondaryValueProperbilityWeightmap = new Dictionary<SecondarValueGenerationTypes, float>
            {
                /// Default Weightmap
                { SecondarValueGenerationTypes.X, 0f },         /// p (x) is not implemented so for now its 0
                { SecondarValueGenerationTypes.One, 0f },
                { SecondarValueGenerationTypes.Specific, 1f },  /// The specific value doesnt work perfectly, so for now its 0
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
            //public enum NumeratorPhase { Bases, Ones, SimpleDirects, Directs, PartlyIndirects };
            //public enum DenominatorPhase { Single, Multiple };

            static List<Fraction> additionFractionList23 = new List<Fraction>()
            {
                new Fraction(1,9),
                new Fraction(2,9),
                //new Fraction(3,9),
                new Fraction(5,9),
                new Fraction(7,9),
                new Fraction(10,9),
                new Fraction(12,9),
                new Fraction(13,9),
                new Fraction(14,9),
                new Fraction(15,9),
                new Fraction(16,9),
                new Fraction(17,9),
                new Fraction(18,9),
                new Fraction(1,8),
                new Fraction(2,8),
                new Fraction(3,8),
                new Fraction(5,8),
                new Fraction(7,8),
                new Fraction(10,8),
                new Fraction(12,8),
                new Fraction(13,8),
                new Fraction(14,8),
                new Fraction(15,8),
                new Fraction(16,8),
                new Fraction(5,4),
                new Fraction(7,4),
                new Fraction(1,6),
                new Fraction(5,6),
                new Fraction(7,6),
                new Fraction(10,6),
            };

            static List<Fraction> additionFractionList235 = new List<Fraction>()
            {
                new Fraction(1,9),
                new Fraction(2,9),
                //new Fraction(3,9),
                new Fraction(7,9),
                //new Fraction(10,9),
                new Fraction(11,9),
                new Fraction(12,9),
                new Fraction(13,9),
                new Fraction(14,9),
                new Fraction(15,9),
                new Fraction(16,9),
                new Fraction(17,9),
                new Fraction(18,9),
                new Fraction(1,8),
                new Fraction(2,8),
                new Fraction(3,8),
                new Fraction(7,8),
                //new Fraction(10,8),
                new Fraction(11,8),
                new Fraction(12,8),
                new Fraction(13,8),
                new Fraction(14,8),
                new Fraction(15,8),
                new Fraction(16,8),
                new Fraction(7,4),
                new Fraction(1,5),
                new Fraction(7,5),
                new Fraction(1,6),
                new Fraction(7,6),
                new Fraction(10,6),
            };

            static List<Fraction> additionFractionList2357 = new List<Fraction>()
            {
                new Fraction(1,9),
                new Fraction(2,9),
                //new Fraction(3,9),
                //new Fraction(7,9),
                //new Fraction(10,9),
                new Fraction(11,9),
                new Fraction(12,9),
                new Fraction(13,9),
                new Fraction(14,9),
                new Fraction(15,9),
                new Fraction(16,9),
                new Fraction(17,9),
                new Fraction(18,9),
                new Fraction(1,8),
                new Fraction(2,8),
                new Fraction(3,8),
                //new Fraction(7,8),
                //new Fraction(10,8),
                new Fraction(11,8),
                new Fraction(12,8),
                new Fraction(13,8),
                new Fraction(14,8),
                new Fraction(15,8),
                new Fraction(16,8),
                //new Fraction(7,4),
                new Fraction(1,5),
                //new Fraction(7,5),
                new Fraction(1,7),
                new Fraction(11,7),
                new Fraction(12,7),
                new Fraction(13,7),
                new Fraction(1,6),
                new Fraction(10,6),
            };

            // Add Dict here (Only Phase included)
            // Addition and Subtraction
            //private static Dictionary<NumeratorPhase, int[]> NumeratorPhaseNumberRef = new Dictionary<NumeratorPhase, int[]>
            //{
            //    { NumeratorPhase.Bases,             new int[] {4,6,8 } },
            //    { NumeratorPhase.Ones,              new int[] {1 } },
            //    { NumeratorPhase.SimpleDirects,     new int[] {2, 3, 5, 7 } },
            //    { NumeratorPhase.Directs,           new int[] {10,12,14,16,17,18 } },
            //    { NumeratorPhase.PartlyIndirects,   new int[] {11,13,15,19,20 } },
            //};

            //private static Dictionary<DenominatorPhase, int[]> DenominatorPhaseNumberRef = new Dictionary<DenominatorPhase, int[]>
            //{
            //    { DenominatorPhase.Single,              new int[] {9 } },
            //    { DenominatorPhase.Multiple,            new int[] {4,8,9 } },
            //};

            /// <summary>
            /// Returns a fraction generated randomly from the exclusivly given phases.
            /// </summary>
            /// <param name="numeratorPhase"></param>
            /// <param name="denominatorPhase"></param>
            static internal Fraction generateEncounterFraction()
            {
                //int numerator;
                //// Take one numerator from Dict
                //int[] numeratorList;
                //// If there are custom denomnumerators set pick those instead
                //if (customNumerators != null)
                //    numeratorList = customNumerators;
                //else
                //    numeratorList = NumeratorPhaseNumberRef[numeratorPhase];

                //int randomIndex;
                //// Adds 9 (Denominator number) to the list of numerators in multiple denominators Phase
                //if (denominatorPhase == DenominatorPhase.Multiple && customNumerators != null)
                //{
                //    // Adds a chance for a 9 into the denominator calculation
                //    randomIndex = UnityEngine.Random.Range(0, numeratorList.Length + 1);
                //    if (randomIndex == numeratorList.Length + 1)
                //    {
                //        numerator = 9;
                //    }
                //    else
                //    {
                //        numerator = numeratorList[randomIndex];
                //    }
                //}
                //else
                //{
                //    // Standard random pick
                //    randomIndex = UnityEngine.Random.Range(0, numeratorList.Length);
                //    numerator = numeratorList[randomIndex];
                //}

                //int denominator;
                //// take one denominator from Dict
                //int[] denominatorList;
                //// If there are custom denominators set pick those instead
                //if (customDenominators != null)
                //    denominatorList = customDenominators;
                //else
                //    denominatorList = DenominatorPhaseNumberRef[denominatorPhase];

                //// Standard random pick
                //randomIndex = UnityEngine.Random.Range(0, denominatorList.Length);
                //denominator = denominatorList[randomIndex];

                //// create a fraction from both
                //Fraction fraction = new Fraction(numerator, denominator);

                Fraction fraction = new Fraction(11,1);

                switch (gameMode)
                {
                    case GameMode.easy23:
                        fraction = GetRandomValueFromList(additionFractionList23);
                        break;
                    case GameMode.medium235:
                        fraction = GetRandomValueFromList(additionFractionList235);
                        break;
                    case GameMode.hard2357:
                        fraction = GetRandomValueFromList(additionFractionList2357);
                        break;
                }

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
            //internal static Fraction generateEncounterFraction()
            //    // Need AdditionAndSubtraction Phase Info since Expandion and Simplifying is just Addition/Subtraction Fractions expanded/simplified
            //{
            //    ExpandingBaseFractionPhase expandingBaseFractionPhase = GetRandomItem(Weights.expandingFractionBaseProperbilityWeightmap);

            //    Fraction fraction;

            //    NumeratorPhase numeratorPhase = GetRandomItem<NumeratorPhase>(Weights.numeratorPhaseProperbilityWeightmap);
            //    DenominatorPhase denominatorPhase = GetRandomItem<DenominatorPhase>(Weights.denominatorPhaseProperbilityWeightmap);
            //    ExpansionPhase expansionPhase = GetRandomItem(Weights.expandingPhaseProperbilityWeightmap);


            //    switch (expandingBaseFractionPhase)
            //    {
            //        // Codeblock for Picking Base Fraction from List of Simplyfied Combined Fractions
            //        case ExpandingBaseFractionPhase.PickFromSimplyfiedCombinedFractions:
            //            fraction = GetRandomValueFromList(SimplyfiedCombinedFractions);
            //            break;
                
            //        // Codeblock for Picking Base Fraction from Addition/Subtraction
            //        case ExpandingBaseFractionPhase.PickFromAdditionSubtraction:
            //            // Since both Single two and three don't include expansions of 4 and 6 in their challanges we have to check for them and instead include a custom number list
            //            switch (expansionPhase)
            //            {
            //                case ExpansionPhase.SingleTwo:
            //                    fraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators: new int[] { 6, 8, 9 });
            //                    break;
            //                case ExpansionPhase.SingleThree:
            //                    fraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase, customDenominators: new int[] { 8, 9 });
            //                    break;
            //                default:
            //                    fraction = AdditionAndSubtraction.generateEncounterFraction(numeratorPhase, denominatorPhase);
            //                    break;
            //            }
            //            // Expand Fraction with value
            //            // Standard random pick, from factors in ref dict
            //            int[] expansionFaktorArray = ExpansionPhaseFactorRef[expansionPhase];
            //            int randomIndex = UnityEngine.Random.Range(0, expansionFaktorArray.Length);
            //            Fraction expandedFraction = fraction.ExpandBy(expansionFaktorArray[randomIndex]);
            //            break;
                
            //        // Failsave
            //        default:
            //            Debug.Log("Could not find ExpandingBaseFractionPhase, returned 11/11 Fraction");
            //            return new Fraction(11, 11);
            //    }

            //    return fraction;
            //}
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

            static List<Fraction> multiplicationSet23 = new List<Fraction>()
            {
                // F2 & N3
                new Fraction(2,27),
                new Fraction(4,27),
                new Fraction(8,27),
                new Fraction(2,81),
                new Fraction(4,81),
                new Fraction(8,81),
                // F3 & N2
                new Fraction(3,16),
                new Fraction(9,16),
                new Fraction(3,32),
                new Fraction(9,32),
                new Fraction(3,64),
                new Fraction(9,64),
            };

            static List<Fraction> simpleMultiplicationSet23 = new List<Fraction>()
            {
                new Fraction(8,27),
                new Fraction(9,16),
            };

            static List<Fraction> multiplicationSet235 = new List<Fraction>()
            {
                // F2 * F3 & N5
                new Fraction(6,25),
                new Fraction(12,25),
                new Fraction(24,25),
                new Fraction(18,25),
                new Fraction(25,36),
                new Fraction(25,72),
                // F2 * F5 & N3
                new Fraction(10,27),
                new Fraction(20,27),
                new Fraction(27,40),
                new Fraction(10,81),
                new Fraction(20,81),
                new Fraction(40,81),
                // F3 & F5 & N2
                new Fraction(15,16),
                new Fraction(16,45),
                new Fraction(15,32),
                new Fraction(32,45),
                new Fraction(15,64),
                new Fraction(45,64),
            };

            static List<Fraction> simpleMultiplicationSet235 = new List<Fraction>()
            {
                new Fraction(9,16),
                new Fraction(5,16),
                new Fraction(15,16),
                new Fraction(2,25),
                new Fraction(4,25),
                new Fraction(8,25),
                new Fraction(16,25),
            };

            static List<Fraction> multiplicationSet2357 = new List<Fraction>()
            {
                // F2 * F3 & F5 * F7
                new Fraction(6,35),
                new Fraction(18,35),
                new Fraction(12,35),
                new Fraction(35,36),
                new Fraction(24,35),
                new Fraction(35,72),
                // F2 * F5 & F3 * F7
                new Fraction(10,21),
                new Fraction(20,21),
                new Fraction(21,40),
                new Fraction(10,63),
                new Fraction(20,63),
                new Fraction(40,63),
                // F2 * F7 & F3 & F5
                new Fraction(14,15),
                new Fraction(15,28),
                new Fraction(15,48),
                new Fraction(14,45),
                new Fraction(28,45),
                new Fraction(45,48),
            };

            static List<Fraction> simpleMultiplicationSet2357 = new List<Fraction>()
            {
                new Fraction(14,15),
                new Fraction(5,16),
                new Fraction(7,16),
                new Fraction(9,16),
                new Fraction(15,16),
                new Fraction(10,21),
                new Fraction(20,21),
                new Fraction(2,25),
                new Fraction(4,25),
                new Fraction(7,25),
                new Fraction(8,25),
                new Fraction(14,25),
                new Fraction(16,25),
            };

            static List<Fraction> multiplication235TwoComposites = new List<Fraction>()
            {
                // Composites: 2,3
                new Fraction(2,27),
                new Fraction(4,27),
                new Fraction(8,27),
                new Fraction(2,81),
                new Fraction(4,81),
                new Fraction(8,81),
                new Fraction(3,16),
                new Fraction(9,16),
                new Fraction(3,32),
                new Fraction(9,32),
                new Fraction(3,64),
                new Fraction(9,64),
                // Composites: 2,5
                new Fraction(2,25),
                new Fraction(4,25),
                new Fraction(8,25),
                new Fraction(5,16),
                new Fraction(5,32),
                new Fraction(5,64),
                // Composites: 3,5
                new Fraction(3,25),
                new Fraction(9,25),
                new Fraction(5,27),
                new Fraction(5,81),
            };

            static List<Fraction> multiplication2357TwoComposites = new List<Fraction>()
            {
                // Composites: 2,3
                new Fraction(2,27),
                new Fraction(4,27),
                new Fraction(8,27),
                new Fraction(2,81),
                new Fraction(4,81),
                new Fraction(8,81),
                new Fraction(3,16),
                new Fraction(9,16),
                new Fraction(3,32),
                new Fraction(9,32),
                new Fraction(3,64),
                new Fraction(9,64),
                // Composites: 2,5
                new Fraction(2,25),
                new Fraction(4,25),
                new Fraction(8,25),
                new Fraction(5,16),
                new Fraction(5,32),
                new Fraction(5,64),
                // Composites: 3,5
                new Fraction(3,25),
                new Fraction(9,25),
                new Fraction(5,27),
                new Fraction(5,81),
                // Composites: 2,7
                new Fraction(2,49),
                new Fraction(4,49),
                new Fraction(8,49),
                new Fraction(7,16),
                new Fraction(7,32),
                new Fraction(7,64),
                // Composites: 3,7
                new Fraction(3,49),
                new Fraction(9,49),
                new Fraction(7,27),
                new Fraction(7,81),
                // Composites: 5,7
                new Fraction(5,49),
                new Fraction(7,25),
            };

            static List<Fraction> multiplication2357ThreeComposites = new List<Fraction>()
            {
                // Composites: 2,3,7
                    // N7
                new Fraction(6,49),
                new Fraction(12,49),
                new Fraction(24,49),
                new Fraction(18,49),
                new Fraction(36,49),
                new Fraction(49,72),
                    // N3
                new Fraction(14,27),
                new Fraction(14,81),
                new Fraction(27,28),
                new Fraction(28,81),
                new Fraction(27,56),
                new Fraction(56,81),
                    // N2
                new Fraction(16,21),
                new Fraction(16,63),
                new Fraction(21,32),
                new Fraction(32,63),
                new Fraction(21,64),
                new Fraction(63,64),
                // Composites: 2,5,7
                    // N7
                new Fraction(10,49),
                new Fraction(20,49),
                new Fraction(40,49),
                    // N5
                new Fraction(14,25),
                new Fraction(25,28),
                new Fraction(25,56),
                    // N2
                new Fraction(16,35),
                new Fraction(32,35),
                new Fraction(35,64),
                // Composites: 3,5,7
                    // N3
                new Fraction(27,35),
                new Fraction(35,81),
                    // N5
                new Fraction(21,25),
                new Fraction(25,63),
                    // N7
                new Fraction(15,49),
                new Fraction(45,49),
            };

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

            public enum ChallangeSet { Normal, Simple, TwoComposites, ThreeComposites }

            public enum SecondarValueGenerationTypes { X, One, Specific }



            // +++++++++++++++++++++++++++++++++++++++++++

            /// <summary>
            /// generates one Encounter Fraction, based on the ChallangeSet-Weightmap. Weightmap can be edited remotely.
            /// </summary>
            /// <param name="make_first_value_the_numerator">Decides how to return the fraction; with the carefully generated value in the numerator or in the denominator. Having this as false will make it harder to solve for the player, but will force them to use multiplication</param>
            /// <returns></returns>
            internal static Fraction generateEncounterFraction(ChallangeSet generationMode)
            {
                Fraction fraction = new Fraction(11,1);

                //int firstValue;
                //int p;

                //// Picks the correct generation based on the GM generationMode
                //switch (generationMode)
                //{
                //    case GM.MD:
                //        firstValue = GetRandomValueFromList(Direkts);
                //        p = P_DIREKTS;
                //        break;
                //    case GM.MiD:
                //        firstValue = GetRandomValueFromList(Indirekts);
                //        p = P_INDIREKTS;
                //        break;
                //    case GM.MiDs:
                //        firstValue = GetRandomValueFromList(Indirekts);
                //        p = P_SUBSTRACT_INDIREKTS;
                //        break;
                //    default:
                //        Debug.LogWarning("Couldn't find the GM in this context. You're likely missing the newly added Challange set in this code block. Returned 11 instead");
                //        firstValue = 11;
                //        p = 1;
                //        break;
                //}

                //Dictionary<int, List<int>> AVAILABLE_BASES = new Dictionary<int, List<int>>()
                //{
                //    { 2 , new List<int>() {2,4,8,16} },
                //    { 3 , new List<int>() {3,9,27} },
                //    { 5 , new List<int>() {5,25} },
                //    { 7 , new List<int>() {7,49} },
                //};

                //List<List<int>> availableBases = new List<List<int>>();

                //switch (gameMode)
                //{
                //    case GameMode.easy23:
                //        availableBases.Add(AVAILABLE_BASES[2]);
                //        availableBases.Add(AVAILABLE_BASES[3]);
                //        break;
                //    case GameMode.medium235:
                //        availableBases.Add(AVAILABLE_BASES[2]);
                //        availableBases.Add(AVAILABLE_BASES[3]);
                //        availableBases.Add(AVAILABLE_BASES[5]);
                //        break;
                //    case GameMode.hard2357:
                //        availableBases.Add(AVAILABLE_BASES[2]);
                //        availableBases.Add(AVAILABLE_BASES[3]);
                //        availableBases.Add(AVAILABLE_BASES[5]);
                //        availableBases.Add(AVAILABLE_BASES[7]);
                //        break;
                //    default:
                //        Debug.LogWarning("Looks like someone didn't set the gamemode in LevelGeneration");
                //        availableBases.Add(AVAILABLE_BASES[2]);
                //        availableBases.Add(AVAILABLE_BASES[3]);
                //        break;
                //}
                //availableBases.FisherYatesShuffle();

                //int z = 1;
                //int n = 1;
                //Debug.Log("12");
                //// Generate Fraction ensuring that both sides are more than 1
                //for (int i = 0; i < availableBases.Count; i++)
                //{
                //    if (i >= 2)
                //    {
                //        int randomOne = GetRandomValueFromList(new List<int> { 0, 1 });
                //        if (randomOne == 0)
                //        {
                //            z *= GetRandomValueFromList(availableBases[i]);
                //            Debug.Log("z3:" + z.ToString());
                //        }
                //        else
                //        {
                //            n *= GetRandomValueFromList(availableBases[i]);
                //            Debug.Log("n3:" + z.ToString());
                //        }
                //    }
                //    else if (i == 1)
                //    {
                //        z *= GetRandomValueFromList(availableBases[i]);
                //        Debug.Log("z:" + z.ToString());
                //    }
                //    else if (i == 0)
                //    {
                //        n *= GetRandomValueFromList(availableBases[i]);
                //        Debug.Log("n:" + z.ToString());
                //    }
                //}

                switch (generationMode)
                {
                    // Standard Multiplication
                    case ChallangeSet.Normal:
                        switch (gameMode)
                        {
                            case GameMode.easy23:
                                fraction = GetRandomValueFromList(multiplicationSet23);
                                break;
                            case GameMode.medium235:
                                fraction = GetRandomValueFromList(multiplicationSet235);
                                break;
                            case GameMode.hard2357:
                                fraction = GetRandomValueFromList(multiplicationSet2357);
                                break;
                        }
                        break;
                    // Simplified Multiplication
                    case ChallangeSet.Simple:
                        switch (gameMode)
                        {
                            case GameMode.easy23:
                                fraction = GetRandomValueFromList(simpleMultiplicationSet23);
                                break;
                            case GameMode.medium235:
                                fraction = GetRandomValueFromList(simpleMultiplicationSet235);
                                break;
                            case GameMode.hard2357:
                                fraction = GetRandomValueFromList(simpleMultiplicationSet2357);
                                break;
                        }
                        break;
                    // Two Composites Multiplication (Default for GameModes with already only Two Composites)
                    case ChallangeSet.TwoComposites:
                        switch (gameMode)
                        {
                            case GameMode.easy23:
                                fraction = GetRandomValueFromList(multiplicationSet23);
                                break;
                            case GameMode.medium235:
                                fraction = GetRandomValueFromList(multiplication235TwoComposites);
                                break;
                            case GameMode.hard2357:
                                fraction = GetRandomValueFromList(multiplication2357TwoComposites);
                                break;
                        }
                        break;
                    // Three Composites Multiplication (Default for GameModes with already only three Composites or less)
                    case ChallangeSet.ThreeComposites:
                        switch (gameMode)
                        {
                            case GameMode.easy23:
                                fraction = GetRandomValueFromList(multiplicationSet23);
                                break;
                            case GameMode.medium235:
                                fraction = GetRandomValueFromList(multiplicationSet235);
                                break;
                            case GameMode.hard2357:
                                fraction = GetRandomValueFromList(multiplication2357ThreeComposites);
                                break;
                        }
                        break;
                }

                //// use a random generation set based on weightmap
                //// for the generation, programm picks from the list of possible numbers to choose from
                //ChallangeSet set = GetRandomItem<ChallangeSet>(Weights.challangeSetProperbilityWeightmap);
                //switch (set)
                //{
                //    case ChallangeSet.Direkt:
                //        firstValue = GetRandomValueFromList(Direkts);
                //        p = P_DIREKTS;
                //        break;
                //    case ChallangeSet.Indirekt:
                //        firstValue = GetRandomValueFromList(Indirekts);
                //        p = P_INDIREKTS;
                //        break;
                //    case ChallangeSet.IndirektSecondDegree:
                //        firstValue = GetRandomValueFromList(IndirektsSecondDegree);
                //        p = P_INDIREKTS_SECOND_DEGREE;
                //        break;
                //    case ChallangeSet.IndirektSubtract:
                //        firstValue = GetRandomValueFromList(Indirekts);
                //        p = P_SUBSTRACT_INDIREKTS;
                //        break;
                //    default:
                //        Debug.LogWarning("Couldn't find the Challange-Set in this context. You're likely missing the newly added Challange set in this code block. Returned 11 instead");
                //        firstValue = 11;
                //        p = 1;
                //        break;
                //}

                //int secondValue = pickSecondaryValue(p);

                //// creates the fraction either with the goal value being the numerator or denominator, based on bool given
                //if (make_first_value_the_numerator)
                //{
                //    fraction = new Fraction(firstValue, secondValue);
                //}
                //else
                //{
                //    fraction = new Fraction(secondValue, firstValue);
                //}

                //if (z > n)
                //{
                //    fraction = new Fraction(n, z);
                //}
                //else
                //{
                //    fraction = new Fraction(z, n);
                //}
                return fraction;

                /// <summary>
                /// Picks a random secondary value based on the weightmap in LevelGeneration and returns it. 
                /// Returns 0, if value should be x (any value works). If you use this function, make sure to cover the 0
                /// </summary>
                /// <param name="p"></param>
                //int pickSecondaryValue(int p)
                //{
                //    SecondarValueGenerationTypes pickRandomValueByWeight(Dictionary<SecondarValueGenerationTypes, float> weightmap)
                //    {
                //        float totalWeight = 0f;
                //        foreach (KeyValuePair<SecondarValueGenerationTypes, float> pair in weightmap)
                //        {
                //            totalWeight += pair.Value;
                //        }

                //        float randomPoint = UnityEngine.Random.Range(0f, totalWeight);

                //        foreach (KeyValuePair<SecondarValueGenerationTypes, float> pair in weightmap)
                //        {
                //            if (randomPoint < pair.Value)
                //            {
                //                return pair.Key;
                //            }
                //            randomPoint -= pair.Value;
                //        }

                //        return default; // Should theoretically never be reached
                //    }

                //    switch (pickRandomValueByWeight(Weights.secondaryValueProperbilityWeightmap))
                //    {
                //        case (SecondarValueGenerationTypes.X):
                //            return 0;
                //        case (SecondarValueGenerationTypes.One):
                //            return 1;
                //        case (SecondarValueGenerationTypes.Specific):
                //            // uses p to generate a secondary value that can be reached using p number of bases
                //            int x = 1;
                //            for (int i = 0; i < p; i++)
                //            {
                //                x *= GetRandomValueFromList(bases);
                //            }
                //            return x;
                //        default:
                //            Debug.LogWarning("Secondary value could not be found. An error in the weightmap or a new ValueGenerationType is likely. Defaulted to 11");
                //            return 11;
                //    }
                //}
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
            List<Fraction> list = generateCardDeckUsingDifficulty(12);

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