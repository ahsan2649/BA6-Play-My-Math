using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board
{
    public class FightButtonComponent : MonoBehaviour, IPointerClickHandler
    {
    
        Canvas _canvas;
        public UnityEvent fightEvent;
        public static FightButtonComponent Instance;
    
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
        
            _canvas = GetComponent<Canvas>();
            
            _canvas.worldCamera = Camera.main;
        }

        private void Start()
        {
            transform.parent.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            fightEvent.Invoke();  
        }

        public void OperationBoardEnableFighting()
        {
            EnableFighting(OperationBoardComponent.Instance.LeftOperand.GetCard()?.GetComponent<NumberCardComponent>()?.Value, OperationBoardComponent.Instance.RightOperand.GetCard()?.GetComponent<NumberCardComponent>()?.Value); 
        }
        
        public void EnableFighting(Fraction leftValue, Fraction rightValue)
        {
            transform.parent.gameObject.SetActive(false);

            if (leftValue is null == rightValue is null)
            {
                return; 
            }
            Fraction toCheckValue = leftValue ?? rightValue; 
            
            foreach (var enemySlot in EnemyZoneComponent.Instance.enemySlots)    
            {
                if (!enemySlot.HasEnemy())
                {
                    continue;
                }

                EnemyComponent enemy = enemySlot.GetEnemy(); 
                // if (enemy.Value.Numerator == toCheckValue.Numerator && enemy.Value.Denominator == toCheckValue.Denominator) //ZyKa!
                if (enemy.Value.EqualsExact(toCheckValue))
                {
                    transform.parent.gameObject.SetActive(true);
                    break; 
                }
            }
        }
    }
}
