using Programming.Fraction_Engine;
using Programming.FractionVisualiser;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.Serialization;

public class OperationBoardVisualiser : MonoBehaviour
{
    [SerializeField] private FractionVisualiser visualiser; 
    [SerializeField] private OperationBoardComponent operationBoardComponent;

    public void UpdateOperationBoardVisuals()
    {
        GetValuesFromOperationBoard();
        visualiser.FullUpdateVisualisations();
    }
    
    private void GetValuesFromOperationBoard()
    {
        visualiser.SetOperation(operationBoardComponent.OperationWheel.currentOperation);
        visualiser.SetSingleFractionValue(operationBoardComponent.LeftOperand.CardInSlot != null ? operationBoardComponent.LeftOperand.CardInSlot.Value : null, OperandType.Left);
        visualiser.SetSingleFractionValue(operationBoardComponent.RightOperand.CardInSlot != null ? operationBoardComponent.RightOperand.CardInSlot.Value : null, OperandType.Right);  
    }
}
