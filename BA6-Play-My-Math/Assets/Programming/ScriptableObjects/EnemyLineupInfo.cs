using System.Collections.Generic;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EnemyLineup", menuName = "ScriptableObjects/Enemy Lineup", order = 1)]
    public class EnemyLineupInfo : ScriptableObject
    {
        public List<Fraction> enemyList;
    }
}
