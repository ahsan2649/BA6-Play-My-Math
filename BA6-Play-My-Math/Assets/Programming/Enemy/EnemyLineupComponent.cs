using System.Collections.Generic;
using Programming.Fraction_Engine;
using Programming.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.Enemy
{
    public class EnemyLineupComponent : MonoBehaviour
    {
        private List<EnemyComponent> _enemies;
        [SerializeField] EnemyLineupInfo enemyLineup;
        [SerializeField] private GameObject enemyPrefab;
        private void OnEnable()
        {
            // Need to populate enemies with Vin's generation algorithm
            _enemies = new List<EnemyComponent>();
            if (enemyLineup != null)
            {
                foreach (Fraction value in enemyLineup.enemyList)
                {
                    var newEnemy = GameObject.Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
                    newEnemy.GetComponent<EnemyComponent>().Value = value;
                    _enemies.Add(newEnemy.GetComponent<EnemyComponent>());
                }
            }
        }

        [ContextMenu("Pop Enemy")]
        public EnemyComponent PopEnemy()
        {
            if (_enemies.Count == 0)
            {
                return null;
            }
            var returningEnemy = _enemies[0];
            _enemies.Remove(returningEnemy);
            return returningEnemy;
        }
    }
}
