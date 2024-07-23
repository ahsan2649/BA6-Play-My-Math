using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/LevelGenerationCondition/PickEnemiesConsecutivePerList")]
    [Tooltip("Mixes all Enemy Lists and Picks enemies from them")]
    public class PickEnemiesConsecutivePerList : TutorialLevelGenerationInfo
    {
        public bool bShuffleAfterPick; 
        
        //no shuffle here
        public override List<Fraction> GenerateEnemyLineup(TutorialLevelInfo levelInfo)
        {
            List<Fraction> combinedEnemies = new List<Fraction>();

            foreach (TutorialLevelInfo.PoolAndCount poolAndCount in levelInfo.enemyPools)
            {
                combinedEnemies = combinedEnemies.Concat(
                    poolAndCount.enemyPoolInfo.PossibleEnemies.GetRandomElementsConsecutively(
                        poolAndCount.enemyCount,
                        poolAndCount.enemyPoolInfo.Probabilities)).ToList(); 
            }

            if (bShuffleAfterPick)
            {
                combinedEnemies.FisherYatesShuffle(); 
            }

            return combinedEnemies; 
        }
    }
}