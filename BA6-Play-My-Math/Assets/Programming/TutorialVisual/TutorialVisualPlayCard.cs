using System;
using System.Collections.Generic;
using System.Linq;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public class TutorialVisualPlayCard : TutorialVisualElement
    {
        [SerializeField] private OperationBoardComponent opBoard;
        [SerializeField] private EnemyZoneComponent enemyZone;
        [SerializeField] private PlayerHandComponent playerHand;
        private Tuple<int, int> matchingIndices;

        public override void Start()
        {
            opBoard = opBoard ? opBoard : OperationBoardComponent.Instance;
            enemyZone = enemyZone ? enemyZone : EnemyZoneComponent.Instance;
            playerHand = playerHand ? playerHand : PlayerHandComponent.Instance; 
            base.Start();
        }

        protected override List<UnityEvent> GetOpenEvents()
        {
            return playerHand.cardSlots.Select(slot => slot.onCardChanged).ToList(); 
        }

        protected override List<UnityEvent> GetCloseEvents()
        { 
            return new List<UnityEvent>() { opBoard.onOperationBoardChange};
        }

        protected override bool CheckCloseCondition()
        {
            return opBoard.LeftOperand.HasCard(); 
        }

        protected override bool CheckOpenCondition()
        {
            return (CheckCardMatchesEnemy(playerHand, enemyZone, out matchingIndices)); 
        }
    }
}
