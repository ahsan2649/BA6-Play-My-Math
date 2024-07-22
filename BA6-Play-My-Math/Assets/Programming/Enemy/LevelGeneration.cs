using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using TMPro;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Programming.Enemy
{
    public class LevelGeneration : MonoBehaviour
    {
        // This is the function accessed externally, handle with care

        public enum GameMode { none, easy23, medium235, hard2357, easyAdditionSmallNumbers }

        public static GameMode gameMode = GameMode.none;

        /// <summary>
        /// Returns a list of Fractions that can be used as the "Monster Deck". 
        /// Make sure you have set the gameMode with switchGameMode(gameMode) before calling this function.
        /// </summary>
        /// <param name="numberOfCards"></param>
        /// <param name="debug_return_test_cue_instead">If true, the function will return a fixed test cue for debugging, instead of trying to generate one. Use it if my code fails to generate a monster deck. </param>
        /// <returns></returns>
        public static List<Fraction> generateEnemyCue(bool debug_return_test_cue_instead = false)
        {
            // If GameMode is not set
            if (gameMode == GameMode.none)
            {
                Debug.LogWarning("The GameMode has not been set in LevelGeneration. Please use the function switchGameMode(gameMode) to set the gameMode first.");

                return new List<Fraction>();
            }

            // Adds to difficulty so every next generation is harder
            currentDifficulty += difficultyAddedEachRound;

            List<Fraction> enemyCue = generateCardDeckUsingDifficulty(currentDifficulty);

            // Generate Deck using my functions
            return enemyCue;
        }

        #region Rewards

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

        static List<Fraction> rewardListAdditionOfSmallNumbers = new List<Fraction>()
        {
            new Fraction(1,1),
            new Fraction(2,1),
            new Fraction(2,1),
            new Fraction(4,1),
            new Fraction(4,1),
            new Fraction(8,1),
            new Fraction(8,1),
            new Fraction(16,1),
        };

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
                    reward = GetRandomValueFromList(rewardList235);
                    break;
                case (GameMode.hard2357):
                    reward = GetRandomValueFromList(rewardList2357);
                    break;
                case (GameMode.easyAdditionSmallNumbers):
                    reward = GetRandomValueFromList(rewardListAdditionOfSmallNumbers);
                    break;
            }

            return reward; 
        }

        ///// <summary>
        ///// This function calculates the reward tupel based on the current difficulty. 
        ///// Call this function BEFORE generating the next EnemyCue with generateEnemyCue.
        ///// The tupel is formated like this: Tupel( cardLeftForOneReward, cardsLeftForTwoRewards, CardsLeftForThreeRewards )
        ///// </summary>
        ///// <returns></returns>
        //public static List<int> generateRewardThresholdValues()
        //{
        //    //List<int> rewardTupel = new List<int> { ((int)(currentDifficulty * 0.1f)), ((int)(currentDifficulty * 0.5f)), currentDifficulty };
        //    List<int> rewardTupel = new List<int> { 2, 4, 6 };

        //    return rewardTupel;
        //}

        #endregion

        private static Dictionary<int, List<List<GM>>> difficultyToGeneration = mainDifficultyToGeneration;

        // Generation Modes
        internal enum GM
        {
            F1,     // B + p = 2
            F1b,    // B + p = 2
            F2,     // B + p = 2
            FB,     // B + p = 2
            FS,     // B + p = 2
            Ads,    // B + p = 4
            AdsB,   // B + p = 4
            AdsE,   // B + p = 4
            Ms,     // B + p = 6
            M,      // B + p = 4
            M2,     // B + p = 4
            M3,     // B + p = 4
        }

        // From this dict generatedFractions are picked. This dict is modified in switchGameMode()
        private static Dictionary<GM, List<Fraction>> Sets = new Dictionary<GM, List<Fraction>>();

        private static List<Fraction> fractionsThatAreOne = new List<Fraction>()
        {
            new Fraction(2,2),
            new Fraction(3,3),
            new Fraction(4,4),
            new Fraction(5,5),
            new Fraction(6,6),
            new Fraction(7,7),
            new Fraction(8,8),
            new Fraction(9,9),
            new Fraction(12,12),
        };


        #region Difficulties

        /// <summary>
        /// Switch to one of the GameModes. This resets the Generation and the enemy cue will be generated from the start.
        /// </summary>
        /// <param name="gameMode"></param>
        public static void switchGameMode(GameMode newGameMode)
        {
            gameMode = newGameMode;
            Debug.Log("Gamemode switched to " + gameMode.ToString());  
            // reset generations
            Sets = new Dictionary<GM, List<Fraction>>();
            // set generations according to gamemode
            switch (gameMode)
            {
                case GameMode.easyAdditionSmallNumbers:
                    // Fractions Sets
                    Sets[GM.FB] = FB_Set_AdditionOfSmallNumbers;
                    Sets[GM.Ads] = Ads_Set_AdditionOfSmallNumbers;
                    Sets[GM.AdsB] = AdsB_Set_AdditionOfSmallNumbers;
                    Sets[GM.AdsE] = AdsE_Set_AdditionOfSmallNumbers;
                    // Generation
                    difficultyToGeneration = difficultyToGenerationEasyAdditionSmallNumbers;
                    maxKeyOfGM = maxKeyAdditionOfSmallNumbers;
                    startDifficulty = startDifficultyAdditionOfSmallNumbers;
                    break;
                case GameMode.easy23:
                    // Fraction Sets
                    Sets[GM.FB] = BaseCombinedFractions23;
                    Sets[GM.FS] = SimplyfiedCombinedFractions23;
                    Sets[GM.Ads] = additionFractionList23;
                    Sets[GM.AdsB] = additionFractionList23Big;
                    Sets[GM.Ms] = simpleMultiplicationSet23;
                    Sets[GM.M] = multiplicationSet23;
                    Sets[GM.M2] = multiplicationSet23;
                    Sets[GM.M3] = multiplicationSet23;
                    // Generation
                    difficultyToGeneration = mainDifficultyToGeneration;
                    maxKeyOfGM = maxKeyOfGMofMain;
                    startDifficulty = startDifficultyMain;
                    break;
                case GameMode.medium235:
                    // Fraction Sets
                    Sets[GM.FB] = BaseCombinedFractions235;
                    Sets[GM.FS] = SimplyfiedCombinedFractions235;
                    Sets[GM.Ads] = additionFractionList235;
                    Sets[GM.AdsB] = additionFractionList235Big;
                    Sets[GM.Ms] = simpleMultiplicationSet235;
                    Sets[GM.M] = multiplicationSet235;
                    Sets[GM.M2] = multiplication235TwoComposites;
                    Sets[GM.M3] = multiplication235TwoComposites;
                    // Generation
                    difficultyToGeneration = mainDifficultyToGeneration;
                    maxKeyOfGM = maxKeyOfGMofMain;
                    startDifficulty = startDifficultyMain;
                    break;
                case GameMode.hard2357:
                    // Fraction Sets
                    Sets[GM.FB] = BaseCombinedFractions2357;
                    Sets[GM.FS] = SimplyfiedCombinedFractions2357;
                    Sets[GM.Ads] = additionFractionList2357;
                    Sets[GM.AdsB] = additionFractionList2357Big;
                    Sets[GM.Ms] = simpleMultiplicationSet2357;
                    Sets[GM.M] = multiplicationSet2357;
                    Sets[GM.M2] = multiplication2357TwoComposites;
                    Sets[GM.M3] = multiplication2357ThreeComposites;
                    // Generation
                    difficultyToGeneration = mainDifficultyToGeneration;
                    maxKeyOfGM = maxKeyOfGMofMain;
                    startDifficulty = startDifficultyMain;
                    break;
            }
            resetDifficulty();
        }

        #region GameMode: Main

        #region Combined Fractions
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
        #endregion

        #region Addition

        internal static List<Fraction> additionFractionList23 = new List<Fraction>()
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

        internal static List<Fraction> additionFractionList235 = new List<Fraction>()
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

        internal static List<Fraction> additionFractionList2357 = new List<Fraction>()
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

        // Addition Fractions that have a bigger numerator than denominator

        internal static List<Fraction> additionFractionList23Big = new List<Fraction>()
            {
                //new Fraction(1,9),
                //new Fraction(2,9),
                //new Fraction(3,9),
                //new Fraction(5,9),
                //new Fraction(7,9),
                new Fraction(10,9),
                new Fraction(12,9),
                new Fraction(13,9),
                new Fraction(14,9),
                new Fraction(15,9),
                new Fraction(16,9),
                new Fraction(17,9),
                new Fraction(18,9),
                //new Fraction(1,8),
                //new Fraction(2,8),
                //new Fraction(3,8),
                //new Fraction(5,8),
                //new Fraction(7,8),
                new Fraction(10,8),
                new Fraction(12,8),
                new Fraction(13,8),
                new Fraction(14,8),
                new Fraction(15,8),
                new Fraction(16,8),
                new Fraction(5,4),
                new Fraction(7,4),
                //new Fraction(1,6),
                //new Fraction(5,6),
                new Fraction(7,6),
                new Fraction(10,6),
            };

        internal static List<Fraction> additionFractionList235Big = new List<Fraction>()
            {
                ////new Fraction(1,9),
                ////new Fraction(2,9),
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
                //new Fraction(1,8),
                //new Fraction(2,8),
                //new Fraction(3,8),
                //new Fraction(7,8),
                //new Fraction(10,8),
                new Fraction(11,8),
                new Fraction(12,8),
                new Fraction(13,8),
                new Fraction(14,8),
                new Fraction(15,8),
                new Fraction(16,8),
                new Fraction(7,4),
                //new Fraction(1,5),
                new Fraction(7,5),
                //new Fraction(1,6),
                new Fraction(7,6),
                new Fraction(10,6),
            };

        internal static List<Fraction> additionFractionList2357Big = new List<Fraction>()
            {
                //new Fraction(1,9),
                //new Fraction(2,9),
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
                //new Fraction(1,8),
                //new Fraction(2,8),
                //new Fraction(3,8),
                //new Fraction(7,8),
                //new Fraction(10,8),
                new Fraction(11,8),
                new Fraction(12,8),
                new Fraction(13,8),
                new Fraction(14,8),
                new Fraction(15,8),
                new Fraction(16,8),
                //new Fraction(7,4),
                //new Fraction(1,5),
                ////new Fraction(7,5),
                //new Fraction(1,7),
                new Fraction(11,7),
                new Fraction(12,7),
                new Fraction(13,7),
                //new Fraction(1,6),
                new Fraction(10,6),
            };
        #endregion

        #region Multiplication

        internal static List<Fraction> multiplicationSet23 = new List<Fraction>()
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

        internal static List<Fraction> simpleMultiplicationSet23 = new List<Fraction>()
            {
                new Fraction(1,27),
                new Fraction(1,16),
                new Fraction(1,32),
                new Fraction(1,36),
            };

        internal static List<Fraction> multiplicationSet235 = new List<Fraction>()
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

        internal static List<Fraction> simpleMultiplicationSet235 = new List<Fraction>()
            {
                //new Fraction(9,16),
                //new Fraction(5,16),
                //new Fraction(15,16),
                //new Fraction(2,25),
                //new Fraction(4,25),
                //new Fraction(8,25),
                //new Fraction(16,25),
                new Fraction(1,27),
                new Fraction(1,16),
                new Fraction(1,32),
                new Fraction(1,36),
                new Fraction(1,25),
                new Fraction(1,40),
                new Fraction(1,20),
            };

        internal static List<Fraction> multiplicationSet2357 = new List<Fraction>()
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

        internal static List<Fraction> simpleMultiplicationSet2357 = new List<Fraction>()
            {
                //new Fraction(14,15),
                //new Fraction(5,16),
                //new Fraction(7,16),
                //new Fraction(9,16),
                //new Fraction(15,16),
                //new Fraction(10,21),
                //new Fraction(20,21),
                //new Fraction(2,25),
                //new Fraction(4,25),
                //new Fraction(7,25),
                //new Fraction(8,25),
                //new Fraction(14,25),
                //new Fraction(16,25),
                new Fraction(1,27),
                new Fraction(1,16),
                new Fraction(1,32),
                new Fraction(1,36),
                new Fraction(1,25),
                new Fraction(1,40),
                new Fraction(1,20),
                new Fraction(1,35),
                new Fraction(1,21),
            };

        internal static List<Fraction> multiplication235TwoComposites = new List<Fraction>()
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

        internal static List<Fraction> multiplication2357TwoComposites = new List<Fraction>()
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

        internal static List<Fraction> multiplication2357ThreeComposites = new List<Fraction>()
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
        #endregion

        // Generation
        private static Dictionary<int, List<List<GM>>> mainDifficultyToGeneration = new Dictionary<int, List<List<GM>>>
        {
            // Intro: Making Fractions
            {6, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB }, 
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB, GM.FB }, 
                }
            },
            // Intro: Simplifying (to 1)
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.F1, GM.F1b },
                }
            },
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.F1, GM.F2 },
                }
            },
            // Intro: Simplified Fractions
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS },
                }
            },
            {21, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.FS },
                }
            },
            {24, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FS, GM.F1b },
                }
            },
            // Wall 1
            {27, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FS, GM.FB, GM.FS, GM.FS, GM.FS, GM.FS }, // 14
                }
            },
            // Intro Ads
            {30, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB },
                }
            },
            {33, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB, GM.Ads },
                }
            },
            {36, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.Ads, GM.FB, GM.FB },
                }
            },
            // Wall 2
            {39, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.FB, GM.Ads, GM.FS, GM.Ads, GM.FS, GM.Ads }, // 24
                }
            },
            // Intro: Multiplication
            {42, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ms }, 
                }
            },
            {45, new List<List<GM>>()
                {
                    new List<GM>() { GM.M },
                }
            },
            {48, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.F1b, GM.M },
                }
            },
            {51, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.Ms, GM.FB, GM.FS, GM.FS },
                }
            },
            // Adding it all together, Final
            {54, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.Ms, GM.Ads, GM.FB, GM.FS, GM.FS },
                }
            },
            {57, new List<List<GM>>()
                {
                    new List<GM>() { GM.M3, GM.Ads, GM.M2, GM.Ads, GM.M2, GM.FB, GM.M3, GM.FS, GM.M, GM.FB, GM.M, GM.Ads, GM.FS, GM.FS }, // 48
                    new List<GM>() { GM.Ads, GM.M, GM.M2, GM.Ads, GM.M3, GM.FB, GM.Ads, GM.FS, GM.M2, GM.FB, GM.Ads, GM.M, GM.FS, GM.FS }, // 48
                }
            },
        };

        private static int maxKeyOfGMofMain = 57;
        private static int startDifficultyMain = 6;

        #endregion

        #region GameMode: AdditionOfSmallNumbers

        // Hand-Cards = 1, 2, 4, 8

        private static List<Fraction> FB_Set_AdditionOfSmallNumbers = new List<Fraction>()
        {
            new Fraction(1,2),
            new Fraction(1,4),
            new Fraction(2,4),
        };

        private static List<Fraction> Ads_Set_AdditionOfSmallNumbers = new List<Fraction>()
        {
            new Fraction(3,4),
            new Fraction(5,4),
            new Fraction(6,4),
        };

        private static List<Fraction> AdsB_Set_AdditionOfSmallNumbers = new List<Fraction>()
        {
            new Fraction(2,8),
            new Fraction(3,8),
            new Fraction(4,8),
            new Fraction(5,8),
            new Fraction(6,8),
        };

        private static List<Fraction> AdsE_Set_AdditionOfSmallNumbers = new List<Fraction>()
        {
            new Fraction(4,16),
            new Fraction(6,16),
            new Fraction(8,16),
            new Fraction(10,16),
            new Fraction(12,16),
        };

        private static Dictionary<int, List<List<GM>>> difficultyToGenerationEasyAdditionSmallNumbers = new Dictionary<int, List<List<GM>>>
        {
            {3, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB },
                }
            },
            {6, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB },
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads },
                }
            },
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.Ads },
                }
            },
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB },
                }
            },
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB, GM.AdsB },
                }
            },
            {21, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsE },
                }
            },
            {24, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsE, GM.AdsE },
                }
            },
            {27, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB, GM.AdsE, GM.Ads, GM.AdsB, GM.AdsE }, // 20
                    new List<GM>() { GM.Ads, GM.FB, GM.AdsB, GM.Ads, GM.Ads, GM.FB }, // 20
                    new List<GM>() { GM.AdsE, GM.AdsE, GM.Ads, GM.FB, GM.AdsB, GM.FB }, // 20
                }
            },
        };

        static int maxKeyAdditionOfSmallNumbers = 27;
        static int startDifficultyAdditionOfSmallNumbers = 3;

        #endregion


        #region Functions: Difficulty Management

        private static int maxKeyOfGM = maxKeyOfGMofMain;
        private static int startDifficulty = startDifficultyMain;

        private static int difficultyAddedEachRound = 3;

        public static int currentDifficulty = startDifficulty - difficultyAddedEachRound;

        /// <summary>
        /// This function is for getting the generation from the corresponding dictonary, making sure that even when exceding the difficuty in the dictonary keys, it still returns a fitting generation.
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
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
                //int rounds = difference % 4;
                //// Make generation list of maximum
                List<GM> gMList = GetRandomValueFromList(difficultyToGeneration[maxKeyOfGM]);
                // Add filler enemy fractions based on how much difficulty
                gMList.AddRange(generateGeneration(difference));
                //for (int i = 0; i < rounds; i++) 
                //{
                //    gMList.Add(GM.M);
                //}
                return gMList;
            }
            Debug.LogWarning("Difficulty was not in difficultyToGeneration, returned empty list instead");
            return new List<GM>();
        }

        /// <summary>
        /// Returns a List of Generations picked at random to fit the given difficulty
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        static List<GM> generateGeneration(int difficulty)
        {
            if (gameMode == GameMode.easy23 || gameMode == GameMode.medium235 || gameMode == GameMode.hard2357)
            {
                // Generation Modes with B + p = 2
                List<GM> generation2 = new List<GM>() { GM.FB, GM.FS };

                // Generation Modes with B + p = 4 (by avarage 5)
                List<GM> generation5 = new List<GM>() { GM.Ads, GM.Ads, GM.M, GM.M2, GM.M3, GM.Ms };

                int rounds = (int)(difficulty / 5);

                List<GM> newGeneration = new List<GM>();

                for (int i = 0; i < rounds; i++)
                {
                    newGeneration.Add(GetRandomValueFromList(generation5));
                }

                for (int i = 0; i < (int)((difficulty - rounds * 5) / 2); i++)
                {
                    newGeneration.Add(GetRandomValueFromList(generation2));
                }

                return newGeneration;
            }
            else if (gameMode == GameMode.easyAdditionSmallNumbers)
            {
                // Generation Modes with B + p = 2
                List<GM> generation2 = new List<GM>() { GM.FB };

                // Generation Modes with B + p = 4
                List<GM> generation5 = new List<GM>() { GM.Ads, GM.AdsB, GM.AdsE };

                int rounds = (int)(difficulty / 4);

                List<GM> newGeneration = new List<GM>();

                for (int i = 0; i < rounds; i++)
                {
                    newGeneration.Add(GetRandomValueFromList(generation5));
                }

                for (int i = 0; i < (int)((difficulty - rounds * 4) / 2); i++)
                {
                    newGeneration.Add(GetRandomValueFromList(generation2));
                }

                return newGeneration;
            }
            
            return new List<GM>();
        }

        /// <summary>
        /// Please call this function whenever the game is started from the beginning. 
        /// This will reset the difficulty and make sure that there is an increasing difficulty for each level
        /// </summary>
        public static void resetDifficulty()
        {
            currentDifficulty = startDifficulty - difficultyAddedEachRound;
        }

        #endregion

        #endregion



        /// <summary>
        /// This is the main function for generating a deck of monster cards.
        /// The generation is based on the weightmaps assigned in the Weights class. Feel free to change their values if you want to change the generation.
        /// </summary>
        /// <param name="numberOfCards"></param>
        /// <returns></returns>
        static List<Fraction> generateCardDeckUsingDifficulty(int difficulty)
        {
            // Create list of Fractions, this represents the deck that will be returned
            List<Fraction> encounterFractions = new List<Fraction>();

            List<GM> generationList = getGenerationFromDifficulty(difficulty);

            Debug.Log("Current Stage Difficulty: " +  difficulty.ToString());

            // for number of cards
            foreach (GM generation in generationList)
            {
                {
                    Fraction encounterFraction;
                    switch (generation)
                    {
                        // Fraction 1,1
                        case GM.F1:
                            encounterFraction = new Fraction(1, 1);
                            encounterFraction.difficulty = 20;
                            encounterFractions.Add(encounterFraction);
                            break;
                        // Fractions that can be simplified to 1,1
                        case GM.F1b:
                            encounterFraction = GetRandomValueFromList(fractionsThatAreOne);
                            encounterFraction.difficulty = 20;
                            encounterFractions.Add(encounterFraction);
                            break;
                        // Fraction 2,1
                        case GM.F2:
                            encounterFraction = new Fraction(2, 1);
                            encounterFraction.difficulty = 20;
                            encounterFractions.Add(encounterFraction);
                            break;
                        // Addition Directs
                        case GM.Ads:
                            encounterFraction = GetRandomValueFromList(Sets[GM.Ads]);
                            encounterFraction.difficulty = 40;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Addition Directs only Big
                        case GM.AdsB:
                            encounterFraction = GetRandomValueFromList(Sets[GM.AdsB]);
                            encounterFraction.difficulty = 40;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Fraction Bases
                        case GM.FB:
                            encounterFraction = GetRandomValueFromList(Sets[GM.FB]);
                            encounterFraction.difficulty = 15;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Fraction Simplified Combines
                        case GM.FS:
                            encounterFraction = GetRandomValueFromList(Sets[GM.FS]);
                            encounterFraction.difficulty = 20;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Directs
                        case GM.Ms:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = GetRandomValueFromList(Sets[GM.Ms]);
                            encounterFraction.difficulty = 55;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Indirects
                        case GM.M:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = GetRandomValueFromList(Sets[GM.M]);
                            encounterFraction.difficulty = 80;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Indirects
                        case GM.M2:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = GetRandomValueFromList(Sets[GM.M2]);
                            encounterFraction.difficulty = 80;
                            encounterFractions.Add(encounterFraction);
                            break;

                        // Multiplication Indirects
                        case GM.M3:
                            /// !!!! Only generates goal values in numerator
                            encounterFraction = GetRandomValueFromList(Sets[GM.M3]);
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
        }


        // These functions make calculating in this script a lot easier, they are private and only for me :3

        private static T GetRandomValueFromList<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentNullException("The list is empty or null.");
            }

            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
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