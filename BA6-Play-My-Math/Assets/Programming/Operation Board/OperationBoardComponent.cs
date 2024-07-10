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
            if (_leftOperand.CardInSlot == null || _rightOperand.CardInSlot == null)
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
            if (_leftOperand.CardInSlot == null || _rightOperand.CardInSlot == null)
            {
                Debug.Log("OpBoard.CalculateCombinedValue: Need two filled slots to calculate!)");
                return null;
            }

            if (_operationWheel.currentOperation == Operation.Nop)
            {
                Debug.Log("OpBoard.CalculateCombinedValue: Operation can't be Nop!");
                return null;
            }

            return Fraction.CalculateOperation(_leftOperand.CardInSlot.Value, _operationWheel.currentOperation, _rightOperand.CardInSlot.Value); 
        }
        
        public void AttackEnemy()
        {
            if (_leftOperand.CardInSlot == null)
            {
                return;
            }

            var attackCardNumber = _leftOperand.CardInSlot.Value;
            foreach (var enemySlot in EnemyZoneComponent.Instance.enemySlots)
            {
                if (!enemySlot.HasEnemy())
                {
                    continue;
                }

                if (attackCardNumber == enemySlot.GetEnemy().Value)
                {
                    var destroyedEnemy = enemySlot.UnsetEnemy();
                    Destroy(destroyedEnemy.gameObject);
                    EnemyZoneComponent.Instance.ZonePush(EnemyLineupComponent.Instance.EnemyPop());

                    var leftCard = _leftOperand._originSlot.UnsetCard();
                    _leftOperand._originSlot = null;
                    _leftOperand.CardInSlot = null;
                    Destroy(leftCard.gameObject);

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
            if (droppedCardNumberComponent == null)
            {
                return;
            }

            if (droppedCardNumberComponent.Value.IsWhole())
            {
                return;
            }

            if (_leftOperand.CardInSlot == null)
            {
                DropCardInSlot(_leftOperand, droppedCard, droppedCardNumberComponent);
                return;
            }
            
            if (_rightOperand.CardInSlot == null)
            {
                DropCardInSlot(_rightOperand, droppedCard, droppedCardNumberComponent);
            }
        }
        #endregion
        
        #region PrivateMethods
        private void DropCardInSlot(OperandSlotComponent operandSlot, GameObject droppedCard,
            NumberCardComponent droppedCardNumberComponent)
        {
            operandSlot._originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            operandSlot.CardInSlot = droppedCardNumberComponent;
            droppedCard.transform.SetParent(operandSlot.transform);

            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().MoveToNewParent());
            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().RotateToNewParent());
        }
        private void SetFinalizedCard(Fraction value)
        {
            var rightCard = _rightOperand._originSlot.UnsetCard();
            _rightOperand._originSlot = null;
            _rightOperand.CardInSlot = null;
            Destroy(rightCard.gameObject);

            _leftOperand.CardInSlot.oldValue = _leftOperand.CardInSlot.Value = value;
            
            // FightButtonComponent.Instance.EnableFighting(value); ZyKa!
            // fractionVisualiser.SetFractionVisualisation(_leftOperand.CardInSlot.Value, FractionVisualiser.VisualisationType.Left); ZyKa!
            // fractionVisualiser.SetFractionVisualisation(null, FractionVisualiser.VisualisationType.Right); ZyKa!
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