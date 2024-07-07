using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Programming.Enemy
{
    public class EnemySlotComponent : MonoBehaviour
    {
        [FormerlySerializedAs("_visualizer")] [SerializeField] private FractionVisualiser visualiser; 
        
        private EnemyComponent _enemyInZone;

        public void SetEnemy(EnemyComponent enemy)
        {
            _enemyInZone = enemy;
            enemy.transform.SetParent(transform);
            enemy.enabled = true;
            visualiser.SetFractionVisualisation(_enemyInZone.Value, FractionVisualiser.VisualisationType.Left);
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