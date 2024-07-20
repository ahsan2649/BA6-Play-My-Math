using System;
using System.Collections.Generic;
using Programming.ExtensionMethods;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Programming.Enemy
{
    public class TutorialLevelAndRewards : MonoBehaviour
    {
        public static TutorialLevelAndRewards Instance;

        #region Subclasses
        [Serializable]
        public class Level
        {
            public string name;
            public List<int> requiredNumbers; //used to check whether players have a chance of beating the level
            public List<Fraction> Rewards => rewards.rewardList; 
            [SerializeField] private RewardCollection rewards;
            public List<Fraction> Enemies => enemies.enemyList; 
            [SerializeField] private EnemyLineupInfo enemies;
            public List<int> rewardThresholds; 
        }
        #endregion

        #region Variable
        [Header("EditorVariables")]
        [SerializeField] private Level[] levels;
    
        [Header("RuntimeVariables")]
        private int _currentLevelNumber;
        private Level _currentLevel; 
        #endregion
    
        private void Awake()
        {
            this.MakeSingleton(ref Instance);
        }

        public void GoToNextLevel()
        {
            _currentLevel = levels[_currentLevelNumber++]; 
        }
        
        public List<Fraction> GenerateEnemyLineup()
        {
            GoToNextLevel();
            return _currentLevel.Enemies; 
        }
        
        public Fraction GenerateEnemy()
        {
            return _currentLevel.Enemies.RandomElement(); 
        }
    
        public Fraction GenerateReward()
        {
            return _currentLevel.Rewards.RandomElement(); 
        }
    }
}
