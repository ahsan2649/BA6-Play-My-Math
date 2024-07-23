using Programming.Card_Mechanism;
using Programming.ScriptableObjects;
using Programming.TutorialVisual;
using UnityEngine;

namespace Programming.TutorialLevel
{
    [CreateAssetMenu(fileName = "TutorialLevel", menuName = "ScriptableObjects/TutorialActions/Modify", order = 4)]
    public class TutorialAction_ActivateModify : TutorialActionInfo
    {
        [SerializeField] private GameObject modifyCardPrefab; 
        
        public override void InitialiseLevel(TutorialLevelInfo levelInfo)
        {
            base.InitialiseLevel(levelInfo);
            DeckComponent.Instance.numberCardPrefab = modifyCardPrefab; 
            DeckComponent.Instance.RebuildDeck();
        }

        public override void StartLevel(TutorialLevelInfo levelInfo)
        {
            base.FinishLevel(levelInfo);
            TutorialVisualManager.Instance.SpawnOrGetTutorial<Tutorial_ModifyCard>();
        }
    }
}