using System;
using UnityEngine;
using UnityEngine.Events;

namespace Programming.Enemy {
    public class EnemyZoneComponent : MonoBehaviour {
        public static EnemyZoneComponent Instance { get; private set; }
        [HideInInspector] public EnemySlotComponent[] enemySlots;

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


        public void InitalizeEnemies()
        {
            for (int i = 0; i < enemySlots.Length; i++)
            {
                ZonePush(EnemyLineupComponent.Instance.EnemyPop());
            }
            StartCoroutine(EnemyLineupComponent.Instance.CascadeEnemies(null));

        }

        public void ZonePush(EnemyComponent enemy)
        {
            

            foreach (EnemySlotComponent slot in enemySlots)
            {
                if (slot.HasEnemy())
                {
                    continue;
                }

                if (enemy != null)
                {
                    slot.SetEnemy(enemy);
                }
                
                break;
            }
            
            
        }

        public bool NoEnemiesLeft()
        {
            if (EnemyLineupComponent.Instance.EnemiesLeft() > 0)
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