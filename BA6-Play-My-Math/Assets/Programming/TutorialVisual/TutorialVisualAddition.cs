using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Operation_Board;
using Programming.Rewards;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public class TutorialVisualAddition : TutorialVisualElement
    {
        protected override List<UnityEvent> GetCloseEvents()
        {
            return new List<UnityEvent>(){ OperationBoardComponent.Instance.onOperationBoardChange }; //Manager.OnFinishLevel, but the GameManager is not a Singleton
        }
        
        protected override bool CheckCloseCondition()
        {
            NumberCardComponent leftNumber = OperationBoardComponent.Instance.LeftOperand.GetNumberCard(); 
            NumberCardComponent rightNumber = OperationBoardComponent.Instance.LeftOperand.GetNumberCard();
            
            if (leftNumber is null || rightNumber is null)
            {
                return false; 
            }
            
            if (leftNumber.Value.Denominator == rightNumber.Value.Denominator)
            {
                return true; 
            }
            
            return false; 
        }
    }
}
