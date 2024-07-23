using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyLineup", menuName = "ScriptableObjects/Enemy/BasicLineup")]
    public class EnemyLineupInfo : ScriptableObject
    {
        public List<Fraction> enemyList;
        public bool randomiseOrder; 
        
        public virtual List<Fraction> GenerateEnemyLineup()
        {
            return randomiseOrder ? enemyList.FisherYatesShuffle() : enemyList; 
        }
    }
}
