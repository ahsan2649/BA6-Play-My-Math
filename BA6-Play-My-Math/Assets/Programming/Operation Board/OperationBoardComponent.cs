using System;
using UnityEngine;

namespace Programming.Operation_Board
{
    public class OperationBoardComponent : MonoBehaviour
    {
        private OperandSlotComponent _leftOperand;
       
        private OperandSlotComponent _rightOperand;
        
        private OperatorWheelComponent _operationWheel;

        private void OnEnable()
        {
            _leftOperand = transform.Find("LeftOperand").GetComponent<OperandSlotComponent>();
            _rightOperand = transform.Find("RightOperand").GetComponent<OperandSlotComponent>();
            _operationWheel = transform.Find("Wheel").GetComponent<OperatorWheelComponent>();
        }

        public void UpdateVisual()
        {
            
        }

        private void OnValidate()
        {
            UpdateVisual();
        }
    }
}