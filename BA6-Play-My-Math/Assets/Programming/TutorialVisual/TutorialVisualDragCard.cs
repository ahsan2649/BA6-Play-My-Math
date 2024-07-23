using System.Collections.Generic;
using System.Linq;
using Programming.Card_Mechanism;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public class TutorialVisualDragCard : TutorialVisualElement
    {
        [SerializeField] private PlayerHandComponent playerHand;

        public override void Start()
        {
            playerHand = playerHand ? playerHand : PlayerHandComponent.Instance;
            base.Start();
            Open(); 
        }

        protected override List<UnityEvent> GetOpenEvents()
        {
            return new List<UnityEvent>(); //actually set directly in inspector
        }

        protected override List<UnityEvent> GetCloseEvents()
        {
            return playerHand.cardSlots.Select(slot => slot.onCardChanged).ToList(); 
        }

        protected override bool CheckCloseCondition()
        {
            foreach (HandSlotComponent slot in playerHand.cardSlots)
            {
                NumberCardComponent numberCard = slot.GetCard()?.GetComponent<NumberCardComponent>(); 
                if (numberCard is not null && numberCard.IsFraction)
                {
                    return true; 
                }
            }
            return false; 
        }
    }
}
