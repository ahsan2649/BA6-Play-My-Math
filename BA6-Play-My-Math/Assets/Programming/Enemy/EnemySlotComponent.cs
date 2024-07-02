using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;

namespace Programming.Enemy
{
    public class EnemySlotComponent : MonoBehaviour
    {
        [SerializeField] private FractionVisualizer _visualizer; 
        
        private EnemyComponent _enemyInZone;

        public void SetEnemy(EnemyComponent enemy)
        {
            _enemyInZone = enemy;
            enemy.transform.SetParent(transform);
            enemy.enabled = true;
            _visualizer.VisualiseFraction(_enemyInZone.Value, FractionVisualisationType.Left);
        }

        public EnemyComponent UnsetEnemy()
        {
            var returningEnemy = _enemyInZone;
            _enemyInZone = null;
            return returningEnemy;
        }

        public EnemyComponent GetEnemy()
        {
            return _enemyInZone;
        }

        public bool HasEnemy() => _enemyInZone != null;
    }
}