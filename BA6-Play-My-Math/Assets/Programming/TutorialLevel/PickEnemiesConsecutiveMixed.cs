using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/LevelGenerationCondition/PickEnemiesConsecutiveMixed")]
    [Tooltip("Mixes all Enemy Lists and Picks enemies from them")]
    public class PickEnemiesConsecutiveMixed : TutorialLevelGenerationInfo
    {
        public override List<Fraction> GenerateEnemyLineup(TutorialLevelInfo levelInfo)
        {
            int enemyCount = levelInfo.enemyPools.Aggregate(0, (count, pool) => count + pool.enemyCount);
            List<Fraction> generatedEnemies = levelInfo.enemyPools.Aggregate(new List<Fraction>(),
                (list, pool) => list.Concat(pool.enemyPoolInfo.PossibleEnemies).ToList());
            List<int> probabilities =  levelInfo.enemyPools.Aggregate(new List<int>(),
                (list, pool) => list.Concat(pool.enemyPoolInfo.Probabilities.Select(probability => probability * pool.enemyCount)).ToList());
            
            return generatedEnemies.GetRandomElementsConsecutively(enemyCount, probabilities);  
        }
    }
}