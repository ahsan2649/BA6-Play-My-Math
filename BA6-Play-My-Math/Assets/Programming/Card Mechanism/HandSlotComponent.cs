using UnityEngine;

namespace Programming.Card_Mechanism
{
    public class HandSlotComponent : SlotComponent
    {
        public bool TryGetNumber(out NumberCardComponent number)
        {
            if (_cardMovementInSlot is null)
            {
                number = null; 
                return false; 
            }

            if (_cardMovementInSlot.TryGetComponent<NumberCardComponent>(out number))
            {
                return true; 
            }

            return false; 
        }
        
        public override void SetCard(CardMovementComponent cardMovement)
        {
            if (_cardMovementInSlot is not null)
            {
                NumberCardComponent oldNumberCard = _cardMovementInSlot.GetComponent<NumberCardComponent>();
                if (oldNumberCard is not null)
                {
                    oldNumberCard.onValueChange.RemoveListener(CallOnCardChanged);
                }
            }
            base.SetCard(cardMovement);
            if (_cardMovementInSlot is not null)
            {
                NumberCardComponent newNumberCard = _cardMovementInSlot.GetComponent<NumberCardComponent>();
                if (newNumberCard is not null)
                {
                    newNumberCard.onValueChange.AddListener(CallOnCardChanged);
                }
            }
        }
    }
}