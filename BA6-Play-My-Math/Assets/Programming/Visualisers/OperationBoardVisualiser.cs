using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using Programming.Visualisers;
using UnityEngine;
using UnityEngine.Serialization;

public class OperationBoardVisualiser : MonoBehaviour
{
    [FormerlySerializedAs("visualiser")] [SerializeField] private FractionVisualizer visualizer; 
    [SerializeField] private OperationBoardComponent operationBoardComponent;

    public void UpdateVisualsFromSingletons()
    {
        GetValuesFromOperationBoard();
        visualizer.FullUpdateVisualisations();
    }
    
    private void GetValuesFromOperationBoard()
    {
        visualizer.SetOperation(operationBoardComponent.OperationWheel.currentOperation);
        visualizer.SetSingleFractionValue(operationBoardComponent.LeftOperand.GetCard() != null ? operationBoardComponent.LeftOperand.GetCard().GetComponent<NumberCardComponent>()?.Value : null, OperandType.Left);
        visualizer.SetSingleFractionValue(operationBoardComponent.RightOperand.GetCard() != null ? operationBoardComponent.RightOperand.GetCard().GetComponent<NumberCardComponent>()?.Value : null, OperandType.Right);  
    }
}
