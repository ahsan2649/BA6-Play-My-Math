using System;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Enemy {
    public class EnemyZoneComponent : MonoBehaviour
    {
        public static EnemyZoneComponent Instance { get; private set; }
        [HideInInspector] public EnemySlotComponent[] enemySlots;
        public UnityEvent LineupComplete;
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
            enemySlots = GetComponentsInChildren<EnemySlotComponent>();
        }


        // Start is called before the first frame update
        void Start()
        {
            enemySlots = GetComponentsInChildren<EnemySlotComponent>();
            
            for (int i = 0; i < enemySlots.Length; i++)
            {
                ZonePush(EnemyLineupComponent.Instance.EnemyPop());
            }
        }

        public void ZonePush(EnemyComponent enemy)
        {
            if (NoEnemiesLeft())
            {
                LineupComplete.Invoke();
                return;
            }
            
            foreach (EnemySlotComponent slot in enemySlots)
            {
                if (slot.HasEnemy())
                {
                    continue;
                }

                if (enemy != null)
                {
                    slot.SetEnemy(enemy);
                    // StartCoroutine(enemy.MoveToNewParent());
                }

                break;
            }
        }

        public bool NoEnemiesLeft()
        {
            if (EnemyLineupComponent.Instance._enemiesInLineup.Count > 0)
            {
                return false;
            }
            foreach (EnemySlotComponent slot in enemySlots)
            {
                if (slot.HasEnemy())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
