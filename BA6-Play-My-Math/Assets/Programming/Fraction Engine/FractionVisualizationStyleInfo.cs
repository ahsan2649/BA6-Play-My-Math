using System.Collections.Generic;
using UnityEngine;

namespace Programming.Fraction_Engine
{
    [CreateAssetMenu(fileName = "EnemyLineup", menuName = "ScriptableObjects/Enemy Lineup", order = 1)]
    public class FractionVisualizationStyleInfo : ScriptableObject
    {
        public Material Material;
        public string _figureName;

        public void ApplyToObject(GameObject gameObject)
        {
            
        }
    }
}