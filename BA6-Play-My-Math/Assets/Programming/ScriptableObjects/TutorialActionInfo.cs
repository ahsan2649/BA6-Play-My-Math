using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.ScriptableObjects
{
    public abstract class TutorialActionInfo : ScriptableObject
    {
        public virtual void InitialiseLevel(TutorialLevelInfo levelInfo)
        {
            
        }
        
        public virtual void StartLevel(TutorialLevelInfo levelInfo)
        {
            
        }

        public virtual void FinishLevel(TutorialLevelInfo levelInfo)
        {
            
        }

        public virtual void UpdateLevel(TutorialLevelInfo levelInfo)
        {
            
        }
    }
}
