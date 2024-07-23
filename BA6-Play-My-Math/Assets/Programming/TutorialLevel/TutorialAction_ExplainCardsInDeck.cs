using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using Programming.TutorialVisual;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/VisualiseCardsInDeck", order = 2)]
    public class TutorialAction_ExplainCardsInDeck : TutorialActionInfo
    {
        public override void StartLevel(TutorialLevelInfo levelInfo)
        {
            base.StartLevel(levelInfo);
            TutorialVisualManager.Instance.SpawnOrGetTutorial<TutorialVisualCardsInDeck>();
        }
    }
}