using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Enemy
{
    public class EnemySlotComponent : MonoBehaviour
    {
        private EnemyComponent _enemyInZone;
        private GameObject changeEnemyValueCanvas; 
        
        public UnityEvent onEnemyChanged; 
        
        public void SetEnemy(EnemyComponent enemy)
        {
            _enemyInZone = enemy;
            enemy.transform.SetParent(transform);
            enemy.enabled = true;
            enemy.onValueChange.AddListener(onEnemyChanged.Invoke);
            StartCoroutine(enemy.MoveToSpot(transform));
            onEnemyChanged.Invoke();
        }
        
        public EnemyComponent UnsetEnemy()
        {
            _enemyInZone.onValueChange.RemoveListener(onEnemyChanged.Invoke);
            var returningEnemy = _enemyInZone;
            _enemyInZone = null;
            onEnemyChanged.Invoke();
            return returningEnemy;
        }
        
        public EnemyComponent GetEnemy()
        {
            return _enemyInZone;
        }

        public bool TryGetEnemy(out EnemyComponent enemyComponent)
        {
            enemyComponent = GetEnemy();
            return enemyComponent is not null; 
        }
        
        public bool HasEnemy() => _enemyInZone != null;
    }
}