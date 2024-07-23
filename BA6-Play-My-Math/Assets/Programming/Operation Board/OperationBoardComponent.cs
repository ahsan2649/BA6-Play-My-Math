using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board
{
    public class OperationBoardComponent : MonoBehaviour, IDropHandler, IEndDragHandler
    {
        public static OperationBoardComponent Instance;
        
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

        [SerializeField] FinalizeButtonComponent finalizeButtonComponent;

        public FinalizeButtonComponent FinalizeButton
        {
            get => finalizeButtonComponent; 
        }
        
        
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

            Fraction combinedValue = CalculateCombinedValue(); 
            if (combinedValue is null || combinedValue < 0)
            {
                return; 
            }
            
            Fraction result = combinedValue; 
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

                EnemyComponent enemy = enemySlot.GetEnemy(); 
                if (attackCardNumber!.EqualsExact(enemy.Value))
                {
                    var destroyedEnemy = enemySlot.UnsetEnemy();
                    Destroy(destroyedEnemy.gameObject);
                    EnemyZoneComponent.Instance.ZonePush(EnemyLineupComponent.Instance.EnemyPop());

                    CardMovementComponent unsetCard = activeOperandSlot.UnsetCard();
                    if (unsetCard is not null) {Destroy(unsetCard.gameObject);} //check in case the player defeats two enemies at once

                    // Send the destroyed enemy to score
                    Score.addFractionToScore(destroyedEnemy.Value);

                    if (!EnemyZoneComponent.Instance.NoEnemiesLeft())
                    {
                        PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
                    }
                    //break; -> we're allowing defeating two exact same enemies at the exact same time
                    OnOperationBoardChangeInvoke();
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
            
            if (_rightOperand.GetCard() == null && _rightOperand.isActiveAndEnabled)
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

            NumberCardComponent leftNumberCard = _leftOperand.GetCard().GetComponent<NumberCardComponent>(); 
            leftNumberCard.oldValue = leftNumberCard.Value = value;
            
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