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
            EnableFighting(operationBoardComponent.CalculateCombinedValue() ??
                           (operationBoardComponent.LeftOperand.CardInSlot != null ?
                               operationBoardComponent.LeftOperand.CardInSlot.Value : 
                            operationBoardComponent.RightOperand.CardInSlot != null ? 
                                operationBoardComponent.RightOperand.CardInSlot.Value :
                                null)); 
        }
        
        public void EnableFighting(Fraction value)
        {
            transform.parent.gameObject.SetActive(false);
            if (value is null)
            {
                return; 
            }
            
            foreach (var enemySlot in EnemyZoneComponent.Instance.enemySlots)    
            {
                if (!enemySlot.HasEnemy())
                {
                    continue;
                }

                if (enemySlot.GetEnemy().Value.Denominator == value.Denominator && enemySlot.GetEnemy().Value.Numerator == value.Numerator)
                {
                    transform.parent.gameObject.SetActive(true);
                    break; 
                }
            }
        }
    }
}
