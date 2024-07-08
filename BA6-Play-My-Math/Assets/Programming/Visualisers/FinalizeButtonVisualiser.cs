using System;
using Programming.Operation_Board;
using UnityEngine;

namespace Programming.Visualisers
{
    public class FinalizeButtonVisualiser : MonoBehaviour
    {
        [SerializeField] private FinalizeButtonComponent finalizeButton;
        [SerializeField] private FractionTextVisualiser fractionTextVisualiser;

        private void Awake()
        {
            fractionTextVisualiser.SetFraction(null);
        }

        public void VisualiseFinaliseButton()
        {
            fractionTextVisualiser.SetFraction(finalizeButton.operationBoardComponent.CalculateCombinedValue());
        }

        public void VisualiseFinaliseButton(FractionTextVisualiser fractionTextVisualiser, OperationBoardComponent operationBoardComponent)
        {
            fractionTextVisualiser.SetFraction(operationBoardComponent.CalculateCombinedValue());
        }
    }
}
