using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.ScriptableObjects
{
    [Serializable]
    public class TutorialLevelInfo
    {
        [Serializable]
        public class PoolAndCount
        {
            [SerializeField] public int enemyCount;
            [FormerlySerializedAs("enemyDirectPoolInfo")] [SerializeField] public EnemyPoolInfo enemyPoolInfo;

            public List<Fraction> GenerateEnemies()
            {
                return enemyPoolInfo.GenerateEnemy(enemyCount); 
            }
        }
        
        public PoolAndCount[] enemyPools;
        public List<int> rewardThresholds;
        public TutorialLevelGenerationInfo specialGenerate;
        
        public List<Fraction> GenerateEnemyLineup()
        {
            if (specialGenerate is not null)
            {
                return specialGenerate.GenerateEnemyLineup(this); 
            }
            else
            {
                return UnmodifiedGenerateEnemy(); 
            } 
        }

        public List<Fraction> UnmodifiedGenerateEnemy()
        {
            List<Fraction> fullEnemyLineup = new List<Fraction>();

            for (int i = 0; i < enemyPools.Length; i++)
            {
                fullEnemyLineup = fullEnemyLineup.Concat(enemyPools[i].GenerateEnemies()).ToList(); 
            }
            fullEnemyLineup.FisherYatesShuffle(); 

            return fullEnemyLineup; 
        }
    }
}
