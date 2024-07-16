using System.Collections.Generic;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Tutorial
{
    public class Tutorial_OperatorWheel : TutorialElement
    {
        [SerializeField] private OperatorWheelComponent operatorWheel;
        [SerializeField] private TutorialElement previousTutorial; 
        
        protected override List<UnityEvent> GetActivationEvents()
        {
            return new List<UnityEvent>(){previousTutorial.onTutorialStepFinished}; 
        }

        protected override List<UnityEvent> GetFinishEvents()
        {
            return new List<UnityEvent>(){operatorWheel.OnChangeOperation}; 
        }

        protected override bool CheckFinishCondition()
        {
            return true; 
        }
    }
}
