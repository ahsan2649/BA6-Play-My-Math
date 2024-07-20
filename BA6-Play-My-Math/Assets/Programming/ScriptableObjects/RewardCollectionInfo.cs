using System.Collections.Generic;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.ScriptableObjects
{
    [CreateAssetMenu(fileName = "RewardCollection", menuName = "ScriptableObjects/RewardCollection", order = 2)]
    public class RewardCollection : ScriptableObject
    {
        [FormerlySerializedAs("rewards")] public List<Fraction> rewardList; 
    }
}
