using System;
using System.Collections;
using System.Collections.Generic;
using Programming.Enemy;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyZoneComponent : MonoBehaviour
{
    public static EnemyZoneComponent Instance { get; private set; }
    [FormerlySerializedAs("cardSlots")] [SerializeField] private EnemySlotComponent[] enemySlots;

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
        
        enemySlots = GetComponentsInChildren<EnemySlotComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
            }

            break;
        }
    }
}
