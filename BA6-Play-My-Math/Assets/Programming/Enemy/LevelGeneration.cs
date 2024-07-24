using System.Collections.Generic;
using Programming.Fraction_Engine;
using Programming.OverarchingFunctionality;
using Programming.ScriptableObjects;
using UnityEngine;

namespace Programming.Enemy
{
    public class LevelGeneration : MonoBehaviour
    {
        private static SceneManaging.GameMode gameMode
        {
            get => SceneManaging.gameMode;
            set => updateGameMode(value); 
        }
        
        // This is the function accessed externally, handle with care
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
            if (gameMode == SceneManaging.GameMode.none)
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
            new Fraction(1,1),
            new Fraction(2,1),
            new Fraction(3,1),
            new Fraction(6,1),
            new Fraction(8,1),
            new Fraction(9,1),
            new Fraction(12,1),
            new Fraction(16,1),
            new Fraction(18,1),

            new Fraction(5,1),
            new Fraction(10,1),
            new Fraction(15,1),
        };

        static List<Fraction> rewardList2357 = new List<Fraction>()
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

            new Fraction(5,1),
            new Fraction(10,1),
            new Fraction(15,1),

            new Fraction(7,1),
            new Fraction(14,1),
            new Fraction(21,1),
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

        public static Fraction GenerateReward(SceneManaging.GameMode gameMode)
        {
            //TODO @Vin: GenerateReward
            Fraction reward = new Fraction(1,1);
            
            switch (gameMode)
            {
                case (SceneManaging.GameMode.easy23):
                    reward = GetRandomValueFromList(rewardList23);
                    break;
                case (SceneManaging.GameMode.medium235):
                    reward = GetRandomValueFromList(rewardList235);
                    break;
                case (SceneManaging.GameMode.hard2357):
                    reward = GetRandomValueFromList(rewardList2357);
                    break;
                // Daily
                case (SceneManaging.GameMode.daily23):
                    reward = GetRandomValueFromList(rewardList23);
                    break;
                case (SceneManaging.GameMode.daily235):
                    reward = GetRandomValueFromList(rewardList235);
                    break;
                case (SceneManaging.GameMode.daily2357):
                    reward = GetRandomValueFromList(rewardList2357);
                    break;
                // Special
                case (SceneManaging.GameMode.easyAdditionSmallNumbers):
                    reward = GetRandomValueFromList(rewardListAdditionOfSmallNumbers);
                    break;
                case (SceneManaging.GameMode.mediumAddition):
                    reward = GetRandomValueFromList(rewardList23);
                    break;
                case (SceneManaging.GameMode.multiplicationOnly):
                    reward = GetRandomValueFromList(rewardList23);
                    break;
                default:
                    reward = new Fraction(1,1);
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
            List<int> rewardList = new List<int> { 2, 4, 6 };

            switch (gameMode)
            {
                // Main
                case SceneManaging.GameMode.easy23:
                    if (rewardThreshholdsMain.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdsMain[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdsMain[maxKeyOfGMofMain];
                    }
                    break;
                case SceneManaging.GameMode.medium235:
                    if (rewardThreshholdsMain.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdsMain[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdsMain[maxKeyOfGMofMain];
                    }
                    break;
                case SceneManaging.GameMode.hard2357:
                    if (rewardThreshholdsMain.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdsMain[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdsMain[maxKeyOfGMofMain];
                    }
                    break;
                
                // Daily
                case SceneManaging.GameMode.daily23:
                    if (rewardThreshholdsDaily.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdsDaily[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdsDaily[maxKeyOfGMofDaily];
                    }
                    break;
                case SceneManaging.GameMode.daily235:
                    if (rewardThreshholdsDaily.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdsDaily[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdsDaily[maxKeyOfGMofDaily];
                    }
                    break;
                case SceneManaging.GameMode.daily2357:
                    if (rewardThreshholdsDaily.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdsDaily[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdsDaily[maxKeyOfGMofDaily];
                    }
                    break;
                
                // Training Modes
                case SceneManaging.GameMode.easyAdditionSmallNumbers:
                    if (rewardThreshholdAdditionSmallNumbers.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdAdditionSmallNumbers[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdAdditionSmallNumbers[maxKeyAdditionOfSmallNumbers];
                    }
                    break;
                case SceneManaging.GameMode.mediumAddition:
                    if (rewardThresholdAdditionMoreNumbers.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThresholdAdditionMoreNumbers[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThresholdAdditionMoreNumbers[maxKeyAdditionOfMoreNumbers];
                    }
                    break;
                case SceneManaging.GameMode.multiplicationOnly:
                    if (rewardThreshholdMultiplicationOnly.ContainsKey(currentDifficulty + difficultyAddedEachRound))
                    {
                        rewardList = rewardThreshholdMultiplicationOnly[currentDifficulty + difficultyAddedEachRound];
                    }
                    // Failsafe and when difficulty ecceeeds dictonary
                    else
                    {
                        rewardList = rewardThreshholdMultiplicationOnly[maxKeyMultiplicationOnly];
                    }
                    break;
                
                default:
                    rewardList = new List<int> { 0, 3, 7, }; break;
            }

            return rewardList;
        }

        #endregion

        private static Dictionary<int, List<List<GM>>> difficultyToGeneration = mainDifficultyToGeneration;

        // Generation Modes
        private enum GM
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
        public static void updateGameMode(SceneManaging.GameMode newGameMode)
        {
            Debug.Log("Gamemode switched to " + gameMode.ToString());
            Score.resetScore();
            // reset generations
            Sets = new Dictionary<GM, List<Fraction>>();
            // set generations according to gamemode
            switch (gameMode)
            {
                // Special Modes
                case SceneManaging.GameMode.easyAdditionSmallNumbers:
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
                case SceneManaging.GameMode.mediumAddition:
                    // Fractions Sets
                    Sets[GM.FB] = FB_Set_AdditionMoreNumbers;
                    Sets[GM.Ads] = Ads_Set_AdditionMoreNumbers;
                    Sets[GM.AdsE] = AdsE_Set_AdditionMoreNumbers;
                    // Generation
                    difficultyToGeneration = difficultyToGenerationEasyAdditionMoreNumbers;
                    maxKeyOfGM = maxKeyAdditionOfMoreNumbers;
                    startDifficulty = startDifficultyAdditionOfMoreNumbers;
                    break;
                case SceneManaging.GameMode.multiplicationOnly:
                    // Fractions Sets
                    Sets[GM.FS] = FS_Set_MultiplicationOnly;
                    Sets[GM.M] = M_Set_MultiplicationOnly;
                    Sets[GM.Ms] = Ms_Set_MultiplicationOnly;
                    // Generation
                    difficultyToGeneration = difficultyToGenerationEasyMultiplicationOnly;
                    maxKeyOfGM = maxKeyMultiplicationOnly;
                    startDifficulty = startDifficultyMultiplicationOnly;
                    break;

                // Main Modes
                case SceneManaging.GameMode.easy23:
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
                case SceneManaging.GameMode.medium235:
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
                case SceneManaging.GameMode.hard2357:
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

                //Daily Mode
                case SceneManaging.GameMode.daily23:
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
                    difficultyToGeneration = dailyDifficultyToGeneration;
                    maxKeyOfGM = maxKeyOfGMofDaily;
                    startDifficulty = startDifficultyDaily;
                    break;
                case SceneManaging.GameMode.daily235:
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
                    difficultyToGeneration = dailyDifficultyToGeneration;
                    maxKeyOfGM = maxKeyOfGMofDaily;
                    startDifficulty = startDifficultyDaily;
                    break;
                case SceneManaging.GameMode.daily2357:
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
                    difficultyToGeneration = dailyDifficultyToGeneration;
                    maxKeyOfGM = maxKeyOfGMofDaily;
                    startDifficulty = startDifficultyDaily;
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

        private static List<Fraction> additionFractionList23 = new List<Fraction>()
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

        private static List<Fraction> additionFractionList235 = new List<Fraction>()
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

        private static List<Fraction> additionFractionList2357 = new List<Fraction>()
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

        private static List<Fraction> additionFractionList23Big = new List<Fraction>()
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

        private static List<Fraction> additionFractionList235Big = new List<Fraction>()
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

        private static List<Fraction> additionFractionList2357Big = new List<Fraction>()
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

        private static List<Fraction> multiplicationSet23 = new List<Fraction>()
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
                // N3 & N2
                new Fraction(16,27),
                new Fraction(27,32),
                new Fraction(27,64),
                new Fraction(16,81),
                new Fraction(32,81),
                new Fraction(64,81),
            };

        private static List<Fraction> simpleMultiplicationSet23 = new List<Fraction>()
            {
                new Fraction(1,27),
                new Fraction(1,16),
                new Fraction(1,32),
                new Fraction(1,36),
            };

        private static List<Fraction> multiplicationSet235 = new List<Fraction>()
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

        private static List<Fraction> simpleMultiplicationSet235 = new List<Fraction>()
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

        private static List<Fraction> multiplicationSet2357 = new List<Fraction>()
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

        private static List<Fraction> simpleMultiplicationSet2357 = new List<Fraction>()
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

        private static List<Fraction> multiplication235TwoComposites = new List<Fraction>()
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

        private static List<Fraction> multiplication2357TwoComposites = new List<Fraction>()
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

        private static List<Fraction> multiplication2357ThreeComposites = new List<Fraction>()
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
                    new List<GM>() { GM.FB, GM.FB },  // 4
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB, GM.FB },  //6
                }
            },
            // Intro: Simplifying (to 1)
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.F1, GM.F1b }, // 4
                }
            },
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.F1, GM.F2 }, // 4
                }
            },
            // Intro: Simplified Fractions
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS }, // 2
                }
            },
            {21, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.FS }, // 4
                }
            },
            {24, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FS, GM.F1b }, // 6
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
                    new List<GM>() { GM.AdsB }, // 4
                }
            },
            {33, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB, GM.Ads }, // 8
                }
            },
            {36, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.Ads, GM.FB, GM.FB }, // 10
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
                    new List<GM>() { GM.Ms }, // 6
                }
            },
            {45, new List<List<GM>>()
                {
                    new List<GM>() { GM.M }, // 6
                }
            },
            {48, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.F1b, GM.M }, // 8
                }
            },
            {51, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.Ms, GM.FB, GM.FS, GM.FS }, // 16
                }
            },
            // Adding it all together, Final
            {54, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.Ms, GM.Ads, GM.FB, GM.FS, GM.FS }, // 20
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

        private static Dictionary<int, List<int>> rewardThreshholdsMain = new Dictionary<int, List<int>>()
        // Start with 10 Hand Cards, each Round +3
        // HandCards - ActualDifficulty = CardsLeftMax
        {
            {6, new List<int>() {0, 2, 4} },    // 10 - 4 = 6      Puffer = 2
            {9, new List<int>() {0, 2, 5} },    // 13 - 6 = 7
            {12, new List<int>() {0, 5, 10} },  // 16 - 4 = 12
            {15, new List<int>() {0, 6, 13} },  // 19 - 4 = 15
            {18, new List<int>() {0, 9, 18} },  // 22 - 2 = 20
            {21, new List<int>() {0, 9, 19} },  // 25 - 4 = 21
            {24, new List<int>() {0, 10, 20} }, // 28 - 6 = 22
            {27, new List<int>() {0, 7, 15} },  // 31 - 14 = 17
            {30, new List<int>() {0, 13, 26} }, // 34 - 4 = 30 (mehr Puffer +2)
            {33, new List<int>() {0, 12, 25} }, // 37 - 8 = 29 (mehr Puffer +2)
            {36, new List<int>() {0, 13, 27} }, // 40 - 10 = 30 (mehr Puffer +1)
            {39, new List<int>() {0, 9, 18} },  // 43 - 24 = 19 (weniger Puffer -1)
            {42, new List<int>() {0, 18, 36} }, // 46 - 6 = 40 (mehr Puffer +2)
            {45, new List<int>() {0, 18, 41} }, // 49 - 4 = 45 (mehr Puffer +2)
            {48, new List<int>() {0, 21, 42} }, // 52 - 8 = 44
            {51, new List<int>() {0, 18, 37} }, // 55 - 16 = 39
            {54, new List<int>() {0, 18, 37} }, // 58 - 20 = 38 (weniger Puffer -1)
            {57, new List<int>() {0, 5, 10} }, // 58 - 48 = 10 (kein Puffer da Ende)
        };
                
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
                    new List<GM>() { GM.FB }, // 2
                }
            },
            {6, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB }, // 4
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads }, // 4
                }
            },
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.Ads }, // 8
                }
            },
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB }, // 4
                }
            },
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB, GM.AdsB }, // 8
                }
            },
            {21, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsE }, // 4
                }
            },
            {24, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsE, GM.AdsE }, // 8
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

        private static Dictionary<int, List<int>> rewardThreshholdAdditionSmallNumbers = new Dictionary<int, List<int>>()
        // Start with 7 Hand Cards, each Round +3
        // HandCards - ActualDifficulty = CardsLeftMax
        {
            {3, new List<int>() {0, 1, 3} },    // 7 - 2 = 5       Puffer = 2
            {6, new List<int>() {0, 2, 4} },    // 10 - 4 = 6      
            {9, new List<int>() {0, 3, 7} },    // 13 - 4 = 9
            {12, new List<int>() {0, 3, 6} },   // 16 - 8 = 8
            {15, new List<int>() {0, 6, 13} },  // 19 - 4 = 15 
            {18, new List<int>() {0, 6, 12} },  // 22 - 8 = 14
            {21, new List<int>() {0, 9, 19} },  // 25 - 4 = 21
            {24, new List<int>() {0, 9, 19} },  // 28 - 8 = 20  (weniger Puffer -1)
            {27, new List<int>() {0, 5, 11} },  // 31 - 20 = 11 (kein Puffer da Ende)
        };

        #endregion

        #region GameMode: AdditionOfMoreNumbers

        // Hand-Cards = 1, 2, 3, 4, 6, 8, 9

        private static List<Fraction> FB_Set_AdditionMoreNumbers = new List<Fraction>()
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

        private static List<Fraction> Ads_Set_AdditionMoreNumbers = new List<Fraction>()
        {
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

        private static List<Fraction> AdsE_Set_AdditionMoreNumbers = new List<Fraction>()
        {
            // Multiplied by 2
            new Fraction(10, 18),  // 5 * 2, 9 * 2
            new Fraction(14, 18),  // 7 * 2, 9 * 2
            new Fraction(20, 18),  // 10 * 2, 9 * 2
            new Fraction(24, 18),  // 12 * 2, 9 * 2
            new Fraction(26, 18),  // 13 * 2, 9 * 2
            new Fraction(28, 18),  // 14 * 2, 9 * 2
            new Fraction(30, 18),  // 15 * 2, 9 * 2
            new Fraction(32, 18),  // 16 * 2, 9 * 2
            new Fraction(34, 18),  // 17 * 2, 9 * 2
            new Fraction(36, 18),  // 18 * 2, 9 * 2
    
            new Fraction(10, 16),  // 5 * 2, 8 * 2
            new Fraction(14, 16),  // 7 * 2, 8 * 2
            new Fraction(20, 16),  // 10 * 2, 8 * 2
            new Fraction(24, 16),  // 12 * 2, 8 * 2
            new Fraction(26, 16),  // 13 * 2, 8 * 2
            new Fraction(28, 16),  // 14 * 2, 8 * 2
            new Fraction(30, 16),  // 15 * 2, 8 * 2
            new Fraction(32, 16),  // 16 * 2, 8 * 2
    
            new Fraction(10, 8),   // 5 * 2, 4 * 2
            new Fraction(14, 8),   // 7 * 2, 4 * 2
    
            new Fraction(2, 12),   // 1 * 2, 6 * 2
            new Fraction(10, 12),  // 5 * 2, 6 * 2
            new Fraction(14, 12),  // 7 * 2, 6 * 2
            new Fraction(20, 12),  // 10 * 2, 6 * 2
    
            // Multiplied by 3
            new Fraction(15, 27),  // 5 * 3, 9 * 3
            new Fraction(21, 27),  // 7 * 3, 9 * 3
            new Fraction(30, 27),  // 10 * 3, 9 * 3
            new Fraction(36, 27),  // 12 * 3, 9 * 3
            new Fraction(39, 27),  // 13 * 3, 9 * 3
            new Fraction(42, 27),  // 14 * 3, 9 * 3
            new Fraction(45, 27),  // 15 * 3, 9 * 3
            new Fraction(48, 27),  // 16 * 3, 9 * 3
            new Fraction(51, 27),  // 17 * 3, 9 * 3
            new Fraction(54, 27),  // 18 * 3, 9 * 3
    
            new Fraction(15, 24),  // 5 * 3, 8 * 3
            new Fraction(21, 24),  // 7 * 3, 8 * 3
            new Fraction(30, 24),  // 10 * 3, 8 * 3
            new Fraction(36, 24),  // 12 * 3, 8 * 3
            new Fraction(39, 24),  // 13 * 3, 8 * 3
            new Fraction(42, 24),  // 14 * 3, 8 * 3
            new Fraction(45, 24),  // 15 * 3, 8 * 3
            new Fraction(48, 24),  // 16 * 3, 8 * 3
    
            new Fraction(15, 12),  // 5 * 3, 4 * 3
            new Fraction(21, 12),  // 7 * 3, 4 * 3
    
            new Fraction(3, 18),   // 1 * 3, 6 * 3
            new Fraction(15, 18),  // 5 * 3, 6 * 3
            new Fraction(21, 18),  // 7 * 3, 6 * 3
            new Fraction(30, 18)   // 10 * 3, 6 * 3
        };

        private static Dictionary<int, List<List<GM>>> difficultyToGenerationEasyAdditionMoreNumbers = new Dictionary<int, List<List<GM>>>
        {
            {6, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB }, // 2
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB }, // 4
                }
            },
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads }, // 4
                }
            },
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.Ads }, // 8
                }
            },
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsE }, // 4
                }
            },
            {21, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsE, GM.AdsE }, // 8
                }
            },
            {24, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB, GM.AdsE, GM.Ads, GM.AdsE, GM.FB }, // 18
                    new List<GM>() { GM.Ads, GM.Ads, GM.FB, GM.AdsE, GM.Ads }, // 18
                    new List<GM>() { GM.AdsE, GM.AdsE, GM.Ads, GM.FB, GM.Ads }, // 18
                }
            },
        };

        static int maxKeyAdditionOfMoreNumbers = 24;
        static int startDifficultyAdditionOfMoreNumbers = 6;

        private static Dictionary<int, List<int>> rewardThresholdAdditionMoreNumbers = new Dictionary<int, List<int>>()
        // Start with 10 Hand Cards, each Round +3
        // HandCards - ActualDifficulty = CardsLeftMax
        {
            {6, new List<int>() {0, 3, 6} },    // 10 - 2 = 8      
            {9, new List<int>() {0, 3, 7} },    // 13 - 4 = 9
            {12, new List<int>() {0, 5, 10} },  // 16 - 4 = 12
            {15, new List<int>() {0, 4, 9} },   // 19 - 8 = 11 
            {18, new List<int>() {0, 8, 16} },  // 22 - 4 = 18
            {21, new List<int>() {0, 8, 16} },  // 25 - 8 = 17  (weniger Puffer -1)
            {24, new List<int>() {0, 5, 10} },  // 28 - 18 = 10 (kein Puffer da Ende)
        };

        // Added a new Difficulty?
        //      Did you add all the new Generation Lists to the switchGameMode()?
        //      Did you add new StartingDeck?
        //      Did you add new Generation after Key?

        #endregion

        #region GameMode: MultiplicationOnly

        // Hand-Cards = 4, 6, 8, 9, 12, 18

        private static List<Fraction> FS_Set_MultiplicationOnly = new List<Fraction>()
        {
            new Fraction(1,3),
            new Fraction(1,2),
            new Fraction(3,4),
            new Fraction(2,3),
        };

        private static List<Fraction> Ms_Set_MultiplicationOnly = new List<Fraction>()
        {
            new Fraction(1,27),
            new Fraction(1,16),
            new Fraction(1,32),
            new Fraction(1,36),
        };

        private static List<Fraction> M_Set_MultiplicationOnly = new List<Fraction>()
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

        private static Dictionary<int, List<List<GM>>> difficultyToGenerationEasyMultiplicationOnly = new Dictionary<int, List<List<GM>>>
        {
            {6, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS }, // 2
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.FS }, // 4
                }
            },
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ms }, // 6
                }
            },
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.M }, // 4
                }
            },
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.M , GM.M, GM.FS}, //10
                    new List<GM>() { GM.M , GM.Ms }, //10
                    new List<GM>() { GM.FS , GM.M, GM.M}, //10
                }
            },
        };

        static int maxKeyMultiplicationOnly = 18;
        static int startDifficultyMultiplicationOnly = 6;

        private static Dictionary<int, List<int>> rewardThreshholdMultiplicationOnly = new Dictionary<int, List<int>>()
        // Start with 10 Hand Cards, each Round +3
        // HandCards - ActualDifficulty = CardsLeftMax
        {
            {6, new List<int>() {0, 3, 6} },    // 10 - 2 = 8      
            {9, new List<int>() {0, 3, 7} },    // 13 - 4 = 9
            {12, new List<int>() {0, 4, 8} },  // 16 - 6 = 10
            {15, new List<int>() {0, 7, 14} },  // 19 - 4 = 15 
            {18, new List<int>() {0, 6, 12} },  // 22 - 10 = 12
        };

        // Added a new Difficulty?
        //      Did you add all the new Generation Lists to the switchGameMode()?
        //      Did you add new StartingDeck?
        //      Did you add new Generation after Key?

        #endregion

        #region GameMode: Daily

        // Generation
        private static Dictionary<int, List<List<GM>>> dailyDifficultyToGeneration = new Dictionary<int, List<List<GM>>>
        {
            {6, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FB }, // 4
                }
            },
            {9, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.FS }, // 4
                }
            },
            {12, new List<List<GM>>()
                {
                    new List<GM>() { GM.FB, GM.FS, GM.FB, GM.FS }, // 8
                }
            },
            {15, new List<List<GM>>()
                {
                    new List<GM>() { GM.AdsB }, // 4
                }
            },
            {18, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ads, GM.AdsB }, // 8
                }
            },
            {21, new List<List<GM>>()
                {
                    new List<GM>() { GM.FS, GM.Ads, GM.FB, GM.FB, GM.FS }, // 12
                }
            },
            {24, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ms }, // 6
                }
            },
            {27, new List<List<GM>>()
                {
                    new List<GM>() { GM.Ms, GM.M }, // 10
                }
            },
            {30, new List<List<GM>>()
                {
                    new List<GM>() { GM.M, GM.Ms, GM.Ads, GM.FB, GM.FS, GM.FS }, // 20
                    new List<GM>() { GM.Ads, GM.M2, GM.M3, GM.FB, GM.FS, GM.FS }, // 20
                    new List<GM>() { GM.M3, GM.M, GM.FS, GM.Ads, GM.FS, GM.FS }, // 20
                    new List<GM>() { GM.FS, GM.M2, GM.Ads, GM.Ads, GM.FS, GM.FS }, // 20
                }
            },
        };

        private static int maxKeyOfGMofDaily = 30;
        private static int startDifficultyDaily = 6;

        private static Dictionary<int, List<int>> rewardThreshholdsDaily = new Dictionary<int, List<int>>()
        // Start with 10 Hand Cards, each Round +3
        // HandCards - ActualDifficulty = CardsLeftMax
        {
            {6, new List<int>() {0, 2, 4} },    // 10 - 4 = 6      Puffer = 2
            {9, new List<int>() {0, 3, 7} },    // 13 - 4 = 9
            {12, new List<int>() {0, 3, 6} },   // 16 - 8 = 8
            {15, new List<int>() {0, 6, 13} },  // 19 - 4 = 15
            {18, new List<int>() {0, 6, 12} },  // 22 - 8 = 14
            {21, new List<int>() {0, 5, 11} },  // 25 - 12 = 13
            {24, new List<int>() {0, 10, 20} }, // 28 - 6 = 22
            {27, new List<int>() {0, 10, 20} }, // 31 - 10 = 21 (weniger Puffer -1)
            {30, new List<int>() {0, 7, 14} },  // 34 - 20 = 14 (kein Puffer da Ende)
        };

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
            if (difficultyToGeneration is null)
            {
                updateGameMode(SceneManaging.gameMode);
            }
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
            if (gameMode == SceneManaging.GameMode.easy23 || gameMode == SceneManaging.GameMode.medium235 || gameMode == SceneManaging.GameMode.hard2357)
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
            else if (gameMode == SceneManaging.GameMode.easyAdditionSmallNumbers)
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
            else if (gameMode == SceneManaging.GameMode.easyAdditionSmallNumbers)
            {
                // Generation Modes with B + p = 2
                List<GM> generation2 = new List<GM>() { GM.FB };

                // Generation Modes with B + p = 4
                List<GM> generation5 = new List<GM>() { GM.Ads, GM.AdsE };

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
            else if (gameMode == SceneManaging.GameMode.multiplicationOnly)
            {
                // Generation Modes with B + p = 2
                List<GM> generation2 = new List<GM>() { GM.FS };

                // Generation Modes with B + p = 4
                List<GM> generation5 = new List<GM>() { GM.M };

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