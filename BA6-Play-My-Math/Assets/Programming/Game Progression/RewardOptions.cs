using System;
using System.Collections;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Programming.GameProgression
{
    /* manually editable list of possible RewardPacks
     * can generate random RewardPacks 
     */
    public class RewardOptions : MonoBehaviour
    {
        [Serializable]
        public class RewardPack
        {
            public ICardable[] rewards; 
        }

        [SerializeField] 
        private List<RewardPack> _possibleRewardPacks;

        [FormerlySerializedAs("SelectableRewards")] public List<RewardPack> SelectableRewardPacks;

        private List<RewardPack> GenerateRandomRewards(int amountRewardOptions)
        {
            SelectableRewardPacks = new List<RewardPack>();
 
            for (int i = 0; i < amountRewardOptions; i++)
            {
                if (amountRewardOptions > _possibleRewardPacks.Count)
                {
                    throw new Exception("amountRewardOptions > _possibleRewardPacks"); 
                }

                RewardPack randomPack = _possibleRewardPacks[Random.Range(0, _possibleRewardPacks.Count)]; 
                SelectableRewardPacks.Add(randomPack);
                _possibleRewardPacks.Remove(randomPack); 
            }

            return SelectableRewardPacks; 
        }

        protected RewardPack SelectRewardPack(RewardPack rewardPack)
        {
            SelectableRewardPacks = null; 
            //TODO: add the rewards to the Deck
            return rewardPack; 
        }
    }
}

