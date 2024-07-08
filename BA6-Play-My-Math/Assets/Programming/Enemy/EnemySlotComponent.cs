using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Programming.Enemy
{
    public class EnemySlotComponent : MonoBehaviour
    {
        private EnemyComponent _enemyInZone;
        
        public UnityEvent onEnemyChanged; 
        
        public void SetEnemy(EnemyComponent enemy)
        {
            _enemyInZone = enemy;
            enemy.transform.SetParent(transform);
            enemy.enabled = true;
            onEnemyChanged.Invoke();
        }
        
        public EnemyComponent UnsetEnemy()
        {
            var returningEnemy = _enemyInZone;
            _enemyInZone = null;
            onEnemyChanged.Invoke();
            return returningEnemy;
        }
        
        public EnemyComponent GetEnemy()
        {
            return _enemyInZone;
        }
        
        public bool HasEnemy() => _enemyInZone != null;
    }
}