using Programming.Operation_Board;
using Programming.ScriptableObjects;
using Programming.TutorialVisual;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/ActivateWheel", order = 6)]
    public class TutorialAction_SetWheelActive : TutorialActionInfo
    {
        public override void StartLevel(TutorialLevelInfo levelInfo)
        {
            OperationBoardComponent.Instance.OperationWheel.SetPlateActive(true);
            OperationBoardComponent.Instance.OperationWheel.SetWheelActive(true);
            
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualOperatorWheel>(); 
            base.StartLevel(levelInfo);
        }
    }
}