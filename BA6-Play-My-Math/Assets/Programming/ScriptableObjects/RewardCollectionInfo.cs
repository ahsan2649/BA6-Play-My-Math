using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RewardCollection", menuName = "ScriptableObjects/RewardCollection/Basic")]
    public class RewardCollectionInfo : ScriptableObject
    {
        [SerializeField] protected List<Fraction> rewardList;

        public virtual Fraction GenerateReward()
        {
            return rewardList.GetRandomElement(); 
        }
    }
}
