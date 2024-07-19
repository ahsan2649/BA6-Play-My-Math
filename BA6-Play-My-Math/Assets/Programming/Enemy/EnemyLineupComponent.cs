using System;
using System.Collections.Generic;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Programming.Enemy
{
    public class EnemyLineupComponent : MonoBehaviour
    {
        public static EnemyLineupComponent Instance { get; private set; }
        
        private List<EnemyComponent> _enemiesInCurrentLineup = new();
        public List<EnemyComponent> EnemiesInCurrentLineup => _enemiesInCurrentLineup;

        [SerializeField] List<EnemyLineupInfo> enemyLineups;
        [SerializeField] private GameObject enemyPrefab;
        
        private int enemyLineUpCounter_temp = 0;

        [SerializeField] private bool bUseAutoEnemyGeneration = true; 
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

        }

        public void CreateEnemyLineup()
        {
            foreach (Fraction value in
                     (enemyLineups == null || enemyLineUpCounter_temp >= enemyLineups.Count || enemyLineups[enemyLineUpCounter_temp].enemyList.Count == 0 || bUseAutoEnemyGeneration)
                         ? LevelGeneration.generateEnemyCue()
                         : enemyLineups[enemyLineUpCounter_temp].enemyList)
            {
                var newEnemy =
                    GameObject.Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
                newEnemy.GetComponent<EnemyComponent>().Value = value;
                newEnemy.GetComponent<EnemyComponent>().UpdateDisplay();
                _enemiesInCurrentLineup.Add(newEnemy.GetComponent<EnemyComponent>());
            }
            
            enemyLineUpCounter_temp += new Random().Next(1, 3); 
            
        }

        [ContextMenu("Pop Enemy")]
        public EnemyComponent EnemyPop()
        {
            if (_enemiesInCurrentLineup.Count == 0)
            {
                return null;
            }

            var returningEnemy = _enemiesInCurrentLineup[0];
            _enemiesInCurrentLineup.Remove(returningEnemy);
            return returningEnemy;
        }
        
        public int EnemiesLeft()
        {
            return _enemiesInCurrentLineup.Count; 
        }
    }
}