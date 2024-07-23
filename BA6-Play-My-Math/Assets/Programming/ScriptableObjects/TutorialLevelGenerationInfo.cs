using System;
using System.Collections.Generic;
using System.Linq;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.ScriptableObjects
{
    public abstract class TutorialLevelGenerationInfo : ScriptableObject
    {
        public virtual List<Fraction> GenerateEnemyLineup(TutorialLevelInfo tutorialLevel)
        {
            return tutorialLevel.UnmodifiedGenerateEnemy(); 
        }
    }
}
