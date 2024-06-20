using System.Collections.Generic;
using UnityEngine;


namespace Programming.Enemy
{
    public class EnemyQueue : MonoBehaviour
    {
        [SerializeField]
        private Queue<Enemy> _enemyQueue;
        public List<Enemy> _activeEnemies;
        
    //PUBLIC
        public void QueueEnemy(Enemy enemy)
        {
            _enemyQueue.Enqueue(enemy);
        }
        
        public Enemy RemoveEnemy(ref Enemy enemy)
        {
            int enemyIndex = -1;
            for (int i = 0; i < _activeEnemies.Count; i++)
            {
                if (_activeEnemies[i] == enemy)
                {
                    return RemoveEnemy(i); 
                }
            }
            
            return null;
        }
    
        public Enemy RemoveEnemy(int index)
        {
            DequeueEnemy(index);
            return _activeEnemies[index]; 
        }
        
    //PRIVATE
        private Enemy DequeueEnemy(int index)
        {
            if (_enemyQueue.Count > 0)
            {
                _activeEnemies[index] = _enemyQueue.Dequeue();
                return _activeEnemies[index]; 
            }
            else if (_activeEnemies.Count == 0)
            {
                //TODO: Win Round    
            }
            
            return null; 
        }
    }
}
