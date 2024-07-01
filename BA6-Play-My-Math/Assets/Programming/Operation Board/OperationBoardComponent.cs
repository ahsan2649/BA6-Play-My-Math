using System;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board
{
    public class OperationBoardComponent : MonoBehaviour, IDropHandler
    {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;

        [SerializeField] OperandSlotComponent _leftOperand;

        [SerializeField] OperandSlotComponent _rightOperand;

        [SerializeField] OperatorWheelComponent _operationWheel;

        [SerializeField] public FractionVisualizer _fractionVisualizer;

        public static OperationBoardComponent Instance;

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

            if (_operationWheel.currentOperation == Operation.Add ||
                _operationWheel.currentOperation == Operation.Subtract)
            {
                if (_leftOperand.CardInSlot.Value.Denominator != _rightOperand.CardInSlot.Value.Denominator)
                {
                    Debug.LogError("Denominators are unequal, can't perform add or subtract");
                    return;
                }

                Fraction result = _operationWheel.currentOperation switch
                {
                    Operation.Add => _leftOperand.CardInSlot.Value + _rightOperand.CardInSlot.Value,
                    Operation.Subtract => _leftOperand.CardInSlot.Value - _rightOperand.CardInSlot.Value,
                    _ => throw new ArgumentOutOfRangeException()
                };
                Debug.Log(result);
                SetFinalizedCard(result);
                return;
            }

            if (_operationWheel.currentOperation == Operation.Multiply ||
                _operationWheel.currentOperation == Operation.Divide)
            {
                Fraction result = _operationWheel.currentOperation switch
                {
                    Operation.Multiply => _leftOperand.CardInSlot.Value * _rightOperand.CardInSlot.Value,
                    Operation.Divide => _leftOperand.CardInSlot.Value / _rightOperand.CardInSlot.Value,
                    _ => throw new ArgumentOutOfRangeException()
                };
                Debug.Log(result);
                SetFinalizedCard(result);
                return;
            }
        }

        void SetFinalizedCard(Fraction value)
        {
            var rightCard = _rightOperand._originSlot.UnsetCard();
            _rightOperand._originSlot = null;
            _rightOperand.CardInSlot = null;
            Destroy(rightCard.gameObject);

            _leftOperand.CardInSlot.oldValue = _leftOperand.CardInSlot.Value = value;

            PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
            Debug.Log(value);
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

        public void DropCardInSlot(OperandSlotComponent operandSlot, GameObject droppedCard,
            NumberCardComponent droppedCardNumberComponent)
        {
            operandSlot._originSlot = droppedCard.GetComponentInParent<HandSlotComponent>();
            operandSlot.CardInSlot = droppedCardNumberComponent;
            droppedCard.transform.SetParent(operandSlot.transform);

            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().MoveToNewParent());
            StartCoroutine(droppedCard.GetComponent<BaseCardComponent>().RotateToNewParent());

            return;
        }
    }
}