using System;
using System.Collections;
using System.Collections.Generic;
using Programming.Operation_Board;
using UnityEngine;

public enum Operation
{
    Nop,
    Add,
    Subtract,
    Multiply,
    Divide,
}

public class OperationBoardComponent : MonoBehaviour
{
    private OperandSlotComponent _leftOperand;
    private OperandSlotComponent _rightOperand;
    private OperatorWheelComponent _operationWheel;
    public Operation currentOperation = Operation.Nop;

    private void OnEnable()
    {
        _leftOperand = transform.Find("LeftOperand").GetComponent<OperandSlotComponent>();
        _rightOperand = transform.Find("RightOperand").GetComponent<OperandSlotComponent>();
        _operationWheel = transform.Find("Wheel").GetComponent<OperatorWheelComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
