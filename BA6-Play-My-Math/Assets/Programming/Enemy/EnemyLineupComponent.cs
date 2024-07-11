using System;
using System.Collections.Generic;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;

namespace Programming.Enemy
{
    public class EnemyLineupComponent : MonoBehaviour
    {
        private List<EnemyComponent> _enemiesInLineup = new();
        [SerializeField] EnemyLineupInfo enemyLineup;
        [SerializeField] private GameObject enemyPrefab;
        public static EnemyLineupComponent Instance { get; private set; }

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
            foreach (Fraction value in
                     (enemyLineup == null || enemyLineup.enemyList.Count == 0)
                         ? LevelGeneration.generateEnemyCue(10)
                         : enemyLineup.enemyList)
            {
                var newEnemy =
                    GameObject.Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
                newEnemy.GetComponent<EnemyComponent>().Value = value;
                newEnemy.GetComponent<EnemyComponent>().UpdateDisplay();
                _enemiesInLineup.Add(newEnemy.GetComponent<EnemyComponent>());
            }
        }

        private void Start()
        {
        }

        [ContextMenu("Pop Enemy")]
        public EnemyComponent EnemyPop()
        {
            if (_enemiesInLineup.Count == 0)
            {
                return null;
            }

            var returningEnemy = _enemiesInLineup[0];
            _enemiesInLineup.Remove(returningEnemy);
            return returningEnemy;
        }
    }
}