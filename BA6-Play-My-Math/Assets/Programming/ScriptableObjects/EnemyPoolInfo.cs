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
    public class FractionProbability
    {
        public FractionProbability(Fraction fraction, int probability)
        {
            this.fraction = new Fraction(fraction);
            this.probability = probability; 
        }
        
        [SerializeField] public Fraction fraction;
        [SerializeField] public int probability = 1; 
    }
    
    [CreateAssetMenu(fileName = "EnemyLineup", menuName = "ScriptableObjects/Enemy/Pool")]
    public class EnemyPoolInfo : ScriptableObject
    {
        [FormerlySerializedAs("possibleEnemies")] [SerializeField] public FractionProbability[] enemiesWithProbability;
        public List<Fraction> PossibleEnemies => enemiesWithProbability.Select(eP => eP.fraction).ToList(); 
        public List<int> Probabilities => enemiesWithProbability.Select(eP => eP.probability).ToList(); 
        public List<Fraction> GenerateEnemy(int count)
        {
            List<Fraction> possibleFraction = enemiesWithProbability.Select(fractionProb => fractionProb.fraction).ToList(); 
            List<int> probabilities = enemiesWithProbability.Select(fractionProb => fractionProb.probability).ToList();
            List<Fraction> generatedList = new List<Fraction>(); 
                
            for (int i = 0; i < count; i++)
            {
                generatedList.Add(possibleFraction.GetRandomElement(probabilities));
            }
            
            return generatedList.FisherYatesShuffle(); 
        }

        [SerializeField] private List<int> numeratorsToFill;
        [SerializeField] private List<int> denominatorsToFill;
        [SerializeField] private int probabilityToSet = 1;  
        [SerializeField] private float MaxValueToFill = 2.0f;
        [SerializeField] private float MinValueToFill = 0.0f; 

        [ContextMenu("FillEntries")]
        public void FillEntries()
        {
            List<FractionProbability> newFractions = new List<FractionProbability>(); 
            foreach (int denominator in denominatorsToFill)
            {
                foreach (int numerator in numeratorsToFill)
                {
                    Fraction fraction = new Fraction(numerator, denominator); 
                    if (fraction >= MinValueToFill && fraction <= MaxValueToFill)
                    {
                        newFractions.Add(new FractionProbability(fraction, probabilityToSet));
                    }
                }
            }

            enemiesWithProbability = enemiesWithProbability.Concat(newFractions).ToArray(); 
        }

        [ContextMenu("ReplaceProbabilities")]
        public void UpdateProbabilitiesFor()
        {
            foreach (int denominator in denominatorsToFill)
            {
                foreach (int numerator in numeratorsToFill)
                {
                    foreach (FractionProbability fractionProbability in enemiesWithProbability)
                    {
                        if (fractionProbability.fraction == (float) numerator/denominator)
                        {
                            fractionProbability.probability = probabilityToSet; 
                        }
                    }
                }
            }
        }

        [ContextMenu("RemoveFractions")]
        public void RemoveFractions()
        {
            List<FractionProbability> toRemove = new List<FractionProbability>();
            foreach (int denominator in denominatorsToFill)
            {
                foreach (int numerator in numeratorsToFill)
                {
                    Fraction fraction = new Fraction(numerator, denominator);
                    if (fraction < MinValueToFill || fraction > MaxValueToFill)
                    {
                        continue; 
                    }
                    
                    foreach (FractionProbability fp in enemiesWithProbability)
                    {
                        if (fp.fraction == fraction)
                        {
                            toRemove.Add(fp); 
                        }
                    }
                }
            }
             
            List<FractionProbability> current = enemiesWithProbability.ToList();
            foreach (FractionProbability fp in toRemove)
            {
                current.Remove(fp); 
            }

            enemiesWithProbability = current.ToArray(); 
        }
    }
}
