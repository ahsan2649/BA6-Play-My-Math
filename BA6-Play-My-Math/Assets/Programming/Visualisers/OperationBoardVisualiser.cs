using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using Programming.Visualisers;
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
        visualiser.SetSingleFractionValue(operationBoardComponent.LeftOperand.GetCard() != null ? operationBoardComponent.LeftOperand.GetCard().GetComponent<NumberCardComponent>()?.Value : null, OperandType.Left);
        visualiser.SetSingleFractionValue(operationBoardComponent.RightOperand.GetCard() != null ? operationBoardComponent.RightOperand.GetCard().GetComponent<NumberCardComponent>()?.Value : null, OperandType.Right);  
    }
}
