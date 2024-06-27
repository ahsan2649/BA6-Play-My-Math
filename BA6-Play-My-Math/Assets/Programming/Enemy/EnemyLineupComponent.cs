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

        private void OnEnable()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            // Need to populate enemies with Vin's generation algorithm
            if (enemyLineup != null)
            {
                foreach (Fraction value in enemyLineup.enemyList)
                {
                    var newEnemy =
                        GameObject.Instantiate(enemyPrefab, transform.position, Quaternion.identity, transform);
                    newEnemy.GetComponent<EnemyComponent>().Value = value;
                    _enemiesInLineup.Add(newEnemy.GetComponent<EnemyComponent>());
                }
            }
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