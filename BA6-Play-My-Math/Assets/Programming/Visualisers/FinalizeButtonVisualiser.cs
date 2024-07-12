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
            fractionTextVisualiser.SetFraction(null, true);
        }

        public void VisualiseFinaliseButton()
        {
            fractionTextVisualiser.SetFraction(finalizeButton.operationBoardComponent.CalculateCombinedValue(), true);
        }

        public void VisualiseFinaliseButton(FractionTextVisualiser textVis, OperationBoardComponent operationBoardComponent)
        {
            textVis.SetFraction(operationBoardComponent.CalculateCombinedValue());
        }
    }
}
