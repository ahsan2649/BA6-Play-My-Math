using System;
using UnityEngine;

namespace Programming.Enemy {
    public class EnemyZoneComponent : MonoBehaviour
    {
        public static EnemyZoneComponent Instance { get; private set; }
        [HideInInspector] public EnemySlotComponent[] enemySlots;

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
        }

        private void Awake()
        {
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
    }
}
