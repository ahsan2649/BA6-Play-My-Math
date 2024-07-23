using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel",
        menuName = "ScriptableObjects/LevelGenerationCondition/EnsureNoDouble")]
    [Tooltip("Mixes all Enemy Lists and Picks enemies from them")]
    public class EnsureNoDouble : TutorialLevelGenerationInfo
    {
        public override List<Fraction> GenerateEnemyLineup(TutorialLevelInfo levelInfo)
        {
            List<Fraction> enemies = levelInfo.UnmodifiedGenerateEnemy();
            int shouldEnemyCount = levelInfo.enemyPools.Aggregate(0, (count, pool) => count + pool.enemyCount);
            int enemyCount = enemies.Distinct(new FractionCompare()).Count(); 
            while (enemyCount < shouldEnemyCount)
            {
                enemies = levelInfo.UnmodifiedGenerateEnemy();
                enemyCount = enemies.Distinct(new FractionCompare()).Count(); 
            }

            return enemies; 
        }


        private class FractionCompare : IEqualityComparer<Fraction>
        {
            public bool Equals(Fraction x, Fraction y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x == y;
            }

            public int GetHashCode(Fraction fraction)
            {
                return fraction.Numerator * 10000 + fraction.Denominator;
            }
        }
    }
}