using UnityEngine;

namespace Programming.Enemy
{
    public class EnemySlotComponent : MonoBehaviour
    {
        private EnemyComponent _enemyInZone;

        public void SetEnemy(EnemyComponent enemy)
        {
            _enemyInZone = enemy;
            enemy.transform.SetParent(transform);
            enemy.enabled = true;
        }

        public EnemyComponent UnsetEnemy(EnemyComponent enemy)
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