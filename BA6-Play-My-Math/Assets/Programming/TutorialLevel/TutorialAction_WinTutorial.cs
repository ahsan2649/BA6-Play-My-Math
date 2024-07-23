using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using Programming.TutorialVisual;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/WinTutorial", order = 2)]
    public class TutorialAction_WinTutorial : TutorialActionInfo
    {
        public override void FinishLevel(TutorialLevelInfo levelInfo)
        {
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualWin>(); 
        }
    }
}