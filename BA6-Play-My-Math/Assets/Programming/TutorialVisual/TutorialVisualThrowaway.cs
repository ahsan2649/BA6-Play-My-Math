using System.Collections.Generic;
using System.Linq;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Programming.TutorialVisual
{
    public class TutorialVisualThrowaway : TutorialVisualElement
    {
        [SerializeField] private EnemyZoneComponent enemyZone; 
        [SerializeField] private PlayerHandComponent playerHand;
        [SerializeField] private OperationBoardComponent opBoard; 
        [FormerlySerializedAs("discardPileComponent")] [SerializeField] private DiscardPileComponent discardPile;

        private int timesCardsThrownAway = 0;
        private float timeLastThrowAway = -5; 
        public float timeTillAutoOpen = 20; 
        private float timerSinceLastAction = 0; 
        
        public override void Start()
        {
            enemyZone = enemyZone ? enemyZone : EnemyZoneComponent.Instance;
            playerHand = playerHand ? playerHand : PlayerHandComponent.Instance;
            discardPile = discardPile ? discardPile : DiscardPileComponent.Instance;
            opBoard = opBoard ? opBoard : OperationBoardComponent.Instance; 
            
            foreach (HandSlotComponent slot in playerHand.cardSlots)
            {
                slot.onCardChanged.AddListener(ResetTimer);
            }
            opBoard.onOperationBoardChange.AddListener(ResetTimer);
            
            base.Start();
        }

        private void OnDestroy()
        {
            foreach (HandSlotComponent slot in playerHand.cardSlots)
            {
                slot.onCardChanged.RemoveListener(ResetTimer);
            }
            opBoard.onOperationBoardChange.RemoveListener(ResetTimer);
            
            discardPile.onDiscardPileChanged.RemoveListener(AddCardsThrownAway); 
        }

        public override void Update()
        {
            timerSinceLastAction += Time.deltaTime;
            if (timerSinceLastAction > timeTillAutoOpen)
            {
                Open();  
                timerSinceLastAction -= timeTillAutoOpen; 
            }
        }

        public void ResetTimer()
        {
            timerSinceLastAction = 0; 
        }

        public void AddCardsThrownAway()
        {
            if (Time.time - timeLastThrowAway > 5 || Time.time - timeLastThrowAway < 0)
            {
                timeLastThrowAway = Time.time; 
                timesCardsThrownAway++;
                if (timesCardsThrownAway >= 3)
                {
                    enabled = false;
                    bFinished = true; 
                }
            }
        }

        protected override List<UnityEvent> GetOpenEvents()
        {
            return new List<UnityEvent>(); //activates via Timer 
        }
        
        protected override bool CheckOpenCondition()
        {
            if (bFinished)
            {
                enabled = false; 
                return false; 
            }
            
            if (timerSinceLastAction < timeTillAutoOpen)
            {
                return false; 
            }

            if (playerHand.cardSlots.Aggregate(0,
                    (numberEmpty, cardSlot) => cardSlot.HasCard() ? numberEmpty : numberEmpty + 1) >= 2) //equivalent to being on the reward-board
            {
                return false; 
            }

            return true; 
            /*
            if ((!CheckEnemyFormable(playerHand, enemyZone, out Tuple<int, int> indices) && timerSinceLastAction >= 10) ||
                (playerHand.cardSlots.Aggregate(0, (numberFraction, slot) => slot.GetNumberCard() is not null && slot.GetNumberCard().IsFraction ? numberFraction+1 : numberFraction) >= 2))
            {
                return false;
            }
            */
        }
        
        protected override List<UnityEvent> GetCloseEvents()
        {
            return playerHand.cardSlots.Select((slot) => slot.onCardChanged).ToList(); 
        }

        protected override bool CheckDeleteCondition()
        {
            return timesCardsThrownAway >= 2; //increase only happens after the Check, thus 2 means 3 here
        }
    }
}
