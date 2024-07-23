using Programming.Operation_Board;
using Programming.ScriptableObjects;
using Programming.TutorialVisual;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/AddSub", order = 5)]
    public class TutorialAction_ActivateAddSub : TutorialActionInfo
    {
        public override void InitialiseLevel(TutorialLevelInfo levelInfo)
        {
            base.InitialiseLevel(levelInfo);
            OperationBoardComponent.Instance.OperationWheel.SetPlateActive(true);
            OperationBoardComponent.Instance.OperationWheel.SetWheelActive(true);
            OperationBoardComponent.Instance.RightOperand.SetActiveAndEnabled(true);
            OperationBoardComponent.Instance.FinalizeButton.SetActiveAndEnabled(true);

            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualAddition>(); 
        }
    }
}