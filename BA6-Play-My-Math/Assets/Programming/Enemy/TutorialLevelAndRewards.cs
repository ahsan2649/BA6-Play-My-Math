using System;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;

namespace Programming.Enemy
{
    public class TutorialLevelAndRewards : MonoBehaviour
    {
        [Serializable]
        public class LevelRewardPair
        {
            public TutorialLevelInfo level;
            public RewardCollectionInfo rewards;
            public TutorialActionInfo tutorialAction; 
        }
        
        public static TutorialLevelAndRewards Instance;

        #region Variable
        
        [Header("EditorVariables")]
        [SerializeField] private LevelRewardPair[] levelsAndRewards;

        public TutorialLevelInfo CurrentLevel => levelsAndRewards[_currentLevelNumber].level; 
        public RewardCollectionInfo CurrentRewards => levelsAndRewards[_currentLevelNumber].rewards; 
        public TutorialActionInfo CurrentTutorialAction => levelsAndRewards[_currentLevelNumber].tutorialAction;
        
        private int _currentLevelNumber = 0;
        private Queue<Fraction> storedRewards = new Queue<Fraction>(); //using this to ensure the rewards are generated at the end of the level, before the level is switched to the next level
        #endregion

        #region StartStopFunctions
        private void Awake()
        {
            this.MakeSingleton(ref Instance);
        }
        
        public void StartCurrentLevel()
        {
            if (CurrentTutorialAction is not null)
            {
                CurrentTutorialAction.StartLevel(CurrentLevel);
            }
        }

        public void Update()
        {
            if (CurrentTutorialAction is not null)
            {
                CurrentTutorialAction.UpdateLevel(CurrentLevel);
            }
        }

        public void FinishLevel()
        {
            if (CurrentTutorialAction is not null)
            {
                CurrentTutorialAction.FinishLevel(CurrentLevel);
            }
        }
        
        public void FinishLevelAndGoNext()
        {
            FinishLevel();
            _currentLevelNumber++; 
        }
        
        public void InitialiseLevel()
        {
            if (CurrentTutorialAction is not null)
            {
                CurrentTutorialAction.InitialiseLevel(CurrentLevel);
            }
        }
        
        #endregion
        
        #region GenerationFunctions
        public List<Fraction> GenerateEnemyLineup()
        {
            return CurrentLevel.GenerateEnemyLineup(); 
        }
        
    
        public Fraction GenerateReward()
        {
            return levelsAndRewards[_currentLevelNumber].rewards.GenerateReward(); 
        }
        
        
        #endregion
    }
}
