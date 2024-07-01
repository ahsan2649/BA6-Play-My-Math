using System;
using Programming.Enemy;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class FightButtonComponent : MonoBehaviour, IPointerClickHandler
{
    RectTransform _rectTransform;
    Canvas _canvas;
    CanvasGroup _canvasGroup;
    public UnityEvent fightEvent;
    public static FightButtonComponent Instance;

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
        
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
            
        _canvas.worldCamera = Camera.main;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        fightEvent.Invoke();  
    }

    public void EnableFighting(Fraction value)
    {
        gameObject.SetActive(false);
        foreach (var enemySlot in EnemyZoneComponent.Instance.enemySlots)    
        {
            if (!enemySlot.HasEnemy())
            {
                continue;
            }

            if (enemySlot.GetEnemy().Value.Denominator == value.Denominator && enemySlot.GetEnemy().Value.Numerator == value.Numerator)
            {
                gameObject.SetActive(true);
                break; 
            }
        }
    }
}
