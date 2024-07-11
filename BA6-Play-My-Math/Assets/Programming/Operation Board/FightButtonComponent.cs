using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board
{
    public class FightButtonComponent : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private OperationBoardComponent operationBoardComponent; 
    
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
            EnableFighting(operationBoardComponent.LeftOperand.CardInSlot?.Value, operationBoardComponent.RightOperand.CardInSlot?.Value); 
        }
        
        public void EnableFighting(Fraction leftValue, Fraction rightValue)
        {
            transform.parent.gameObject.SetActive(false);

            if (leftValue is null && rightValue is null ||
                leftValue is not null && rightValue is not null)
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

                if (enemySlot.GetEnemy().Value == toCheckValue)
                {
                    transform.parent.gameObject.SetActive(true);
                    break; 
                }
            }
        }
    }
}
