using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Programming.Fraction_Engine;
using Programming.OverarchingFunctionality;
using Programming.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Programming.Enemy
{
    public enum EnemyGenerationType
    {
        Tutorial, 
        Roguelite, 
    }
    
    public class EnemyLineupComponent : MonoBehaviour
    {
        public static EnemyLineupComponent Instance { get; private set; }
     
        [Header("References")]
        [SerializeField] private GameObject enemyPrefab;
        
        [Header("EditorVariables")]
        [SerializeField] List<EnemyLineupInfo> enemyLineups;
        [SerializeField] private bool bUseAutoEnemyGeneration = true;
        [SerializeField] private EnemyGenerationType enemyGenerationType; 
        
        [Header("RuntimeVariables")]
        
        private int enemyLineUpCounter_temp = 0;
        public List<EnemyComponent> EnemiesInCurrentLineup => _enemiesInCurrentLineup;
        private List<EnemyComponent> _enemiesInCurrentLineup = new();

        public List<Transform> LineupSpots;
        public Transform SpawnPoint;

        public UnityEvent InitializeEvent;
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
            List<Fraction> enemyFractionList = enemyGenerationType switch
            {
                EnemyGenerationType.Tutorial => TutorialLevelAndRewards.Instance.GenerateEnemyLineup(),
                EnemyGenerationType.Roguelite => LevelGeneration.generateEnemyCue(),
                _ => throw new SwitchExpressionException()
            }; 
            
            foreach (Fraction value in enemyFractionList)
            {
                var newEnemy =
                    GameObject.Instantiate(enemyPrefab, SpawnPoint.position, enemyPrefab.transform.rotation, transform);
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

        public IEnumerator CascadeEnemies(UnityEvent next)
        {
            for (var index = 0; index < LineupSpots.Count; index++)
            {
                if (index >= EnemiesInCurrentLineup.Count)
                {
                    break;
                }
                var spot = LineupSpots[index];
                var enemy = EnemiesInCurrentLineup[index];

                StartCoroutine(enemy.MoveToSpot(spot));
                yield return new WaitForSeconds(.5f);
            }
            
            yield return new WaitForSeconds(1f);

            if (next is not null)
            {
                next.Invoke();
            }
        }

        public void StartLineup()
        {
            StartCoroutine(CascadeEnemies(InitializeEvent));
        }
    }
}