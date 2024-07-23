using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RewardCollection", menuName = "ScriptableObjects/RewardCollection/Probabilities")]
    public class RewardCollectionProbabilityInfo : RewardCollectionInfo
    {
        [SerializeField] private int[] probabilities; 
        
        public override Fraction GenerateReward()
        {
            return rewardList.GetRandomElement(probabilities); 
        }
    }
}
