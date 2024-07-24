using Programming.Operation_Board;
using Programming.ScriptableObjects;
using Programming.TutorialVisual;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/WinTutorial", order = 2)]
    public class TutorialAction_WinTutorial : TutorialActionInfo
    {
        public override void StartLevel(TutorialLevelInfo levelInfo)
        {
            OperationBoardComponent.Instance.OperationWheel.SetWheelActive(true);
        }
        
        public override void FinishLevel(TutorialLevelInfo levelInfo)
        {
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualWin>(); 
        }
    }
}