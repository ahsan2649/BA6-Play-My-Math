using System.Collections.Generic;
using Programming.Rewards;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public class TutorialVisualSelectReward : TutorialVisualElement
    {
        [SerializeField] private RewardBoardComponent rewardBoard;

        public override void Start()
        {
            rewardBoard = rewardBoard ? rewardBoard : RewardBoardComponent.Instance; 
            base.Start();
        }

        protected override List<UnityEvent> GetOpenEvents()
        {
            return new List<UnityEvent>(){rewardBoard.onRewardsSpawned};
        }

        protected override List<UnityEvent> GetCloseEvents()
        {
            return new List<UnityEvent>(){rewardBoard.onBoardStartExit};
        }
    }
}
