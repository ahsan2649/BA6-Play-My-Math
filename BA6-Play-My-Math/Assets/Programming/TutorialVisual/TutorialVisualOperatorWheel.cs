using System.Collections.Generic;
using Programming.Operation_Board;
using UnityEngine.Events;

namespace Programming.TutorialVisual
{
    public class TutorialVisualOperatorWheel : TutorialVisualElement
    {
        protected override List<UnityEvent> GetCloseEvents()
        {
            return new List<UnityEvent>(){OperationBoardComponent.Instance.OperationWheel.OnChangeOperation}; 
        }

        protected override bool CheckCloseCondition()
        {
            return true; 
        }
    }
}
