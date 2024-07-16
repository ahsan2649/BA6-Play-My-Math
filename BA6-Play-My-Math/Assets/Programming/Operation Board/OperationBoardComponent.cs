using System;
using System.Runtime.CompilerServices;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Programming.Operation_Board
{
    public class OperationBoardComponent : MonoBehaviour, IDropHandler, IEndDragHandler
    {
        Canvas _canvas;
        
        [SerializeField] OperandSlotComponent _leftOperand;
        public OperandSlotComponent LeftOperand
        {
            get => _leftOperand; 
        }
        
        [SerializeField] OperandSlotComponent _rightOperand;
        public OperandSlotComponent RightOperand
        {
            get => _rightOperand; 
        }
        
        [SerializeField] OperatorWheelComponent _operationWheel;

        public OperatorWheelComponent OperationWheel
        {
            get => _operationWheel; 
        }
        
        
        public static OperationBoardComponent Instance;

        public UnityEvent onOperationBoardChange; 
        
        #region MonoBehaviours
        private void Awake() //must run before any OnEnable or Start, so that Instance is set, when other Object search for in in their OnEnable or Start
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
        #endregion
        
        #region publicFunctions

        public void OnOperationBoardChangeInvoke()
        {
            onOperationBoardChange.Invoke();
        }
        
        public void FinalizeOperation()
        {
            if (_leftOperand.GetCard() == null || _rightOperand.GetCard() == null)
            {
                Debug.LogError("Need two filled slots to calculate!");
                return;
            }

            if (_operationWheel.currentOperation == Operation.Nop)
            {
                Debug.LogError("Operation can't be Nop!");
                return;
            }

            Fraction result = CalculateCombinedValue(); 
            Debug.Log(result);
            SetFinalizedCard(result);

            onOperationBoardChange.Invoke(); 
        }

        public Fraction CalculateCombinedValue()
        {
            if (_leftOperand.GetCard() == null || _rightOperand.GetCard() == null)
            {
                Debug.Log("OpBoard.CalculateCombinedValue: Need two filled slots to calculate!)");
                return null;
            }

            if (_operationWheel.currentOperation == Operation.Nop)
            {
                Debug.Log("OpBoard.CalculateCombinedValue: Operation can't be Nop!");
                return null;
            }

            return Fraction.CalculateOperation(_leftOperand.GetCard().GetComponent<NumberCardComponent>()?.Value, _operationWheel.currentOperation, _rightOperand.GetCard().GetComponent<NumberCardComponent>()?.Value); 
        }
        
        public void AttackEnemy()
        {
            if (_leftOperand.GetCard() is null == _rightOperand.GetCard() is null)
            {
                return;
            }

            var activeOperandSlot = _leftOperand.GetCard() is not null ? _leftOperand : _rightOperand;
            var activeCard = activeOperandSlot.GetCard(); 
            var attackCardNumber = activeCard.GetComponent<NumberCardComponent>()?.Value;
            foreach (var enemySlot in EnemyZoneComponent.Instance.enemySlots)
            {
                if (!enemySlot.HasEnemy())
                {
                    continue;
                }

                if (attackCardNumber.Numerator == enemySlot.GetEnemy().Value.Numerator && attackCardNumber.Denominator == enemySlot.GetEnemy().Value.Denominator)
                {
                    var destroyedEnemy = enemySlot.UnsetEnemy();
                    Destroy(destroyedEnemy.gameObject);
                    EnemyZoneComponent.Instance.ZonePush(EnemyLineupComponent.Instance.EnemyPop());

                    CardMovementComponent unsetCard = activeOperandSlot.UnsetCard();
                    Destroy(unsetCard.gameObject);
                    
                    PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            var droppedCard = eventData.pointerDrag;
            if (droppedCard == null)
            {
                return;
            }

            var droppedCardNumberComponent = droppedCard.GetComponent<NumberCardComponent>();
            var droppedCardMovementComponent = droppedCard.GetComponent<CardMovementComponent>();
            if (droppedCardNumberComponent == null)
            {
                return;
            }

            if (!droppedCardNumberComponent.IsFraction)
            {
                return;
            }

            if (_leftOperand.GetCard() == null)
            {
                DropCardInSlot(_leftOperand, droppedCard, droppedCardMovementComponent);
                return;
            }
            
            if (_rightOperand.GetCard() == null)
            {
                DropCardInSlot(_rightOperand, droppedCard, droppedCardMovementComponent);
            }
        }
        #endregion
        
        #region PrivateMethods
        private void DropCardInSlot(OperandSlotComponent operandSlot, GameObject droppedCard,
            CardMovementComponent droppedCardMovementComponent)
        {
            operandSlot.SwapCards(droppedCardMovementComponent.currentSlot, droppedCardMovementComponent);
            return;
        }
        private void SetFinalizedCard(Fraction value)
        {
            CardMovementComponent rightCard = _rightOperand.UnsetCard();
            Destroy(rightCard.gameObject);

            _leftOperand.GetCard().GetComponent<NumberCardComponent>().oldValue = _leftOperand.GetCard().GetComponent<NumberCardComponent>().Value = value;
            
            onOperationBoardChange.Invoke();

            PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
            Debug.Log(value);
        }
        #endregion

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("EndDrag on OperationBoard");
        }
    }
}