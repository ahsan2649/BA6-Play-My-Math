using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Visualisers
{
    public class FinalizeButtonVisualiser : MonoBehaviour
    {
        [SerializeField] private FinalizeButtonComponent finalizeButton;
        [SerializeField] private FractionTextVisualiser fractionTextVisualiser;
        [SerializeField] private Animator animator; 
        private static readonly int IsFinalizeable = Animator.StringToHash("isFinalizeable");
        

        private void Awake()
        {
            fractionTextVisualiser.SetFraction(null, true);
        }

        public void VisualiseFinaliseButtonFromSingletons()
        {
            VisualiseFinaliseButton(fractionTextVisualiser, OperationBoardComponent.Instance);
        }

        public void VisualiseFinaliseButton(FractionTextVisualiser textVis, OperationBoardComponent operationBoardComponent)
        {
            Fraction combinedFraction = operationBoardComponent.CalculateCombinedValue(); 
            textVis.SetFraction(operationBoardComponent.CalculateCombinedValue());
            animator.SetBool(IsFinalizeable, combinedFraction is not null);
        }
    }
}
