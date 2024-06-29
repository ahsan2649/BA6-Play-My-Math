using System;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;
using UnityEngine;

namespace Programming.Operation_Board
{
    public class OperationBoardComponent : MonoBehaviour
    {
        [SerializeField] OperandSlotComponent _leftOperand;

        [SerializeField] OperandSlotComponent _rightOperand;

        [SerializeField] OperatorWheelComponent _operationWheel;

        [SerializeField] FractionVisualizer _fractionVisualizer;

        private void OnEnable()
        {

        }
        
        public void FinalizeOperation()
        {
            if (_leftOperand._cardInSlot == null || _rightOperand._cardInSlot == null)
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
                if (_leftOperand._cardInSlot.Value.Denominator != _rightOperand._cardInSlot.Value.Denominator)
                {
                    Debug.LogError("Denominators are unequal, can't perform add or subtract");
                    return;
                }

                Fraction result = _operationWheel.currentOperation switch
                {
                    Operation.Add => _leftOperand._cardInSlot.Value + _rightOperand._cardInSlot.Value,
                    Operation.Subtract => _leftOperand._cardInSlot.Value - _rightOperand._cardInSlot.Value,
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
                    Operation.Multiply => _leftOperand._cardInSlot.Value * _rightOperand._cardInSlot.Value,
                    Operation.Divide => _leftOperand._cardInSlot.Value / _rightOperand._cardInSlot.Value,
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
            _rightOperand._cardInSlot = null;
            Destroy(rightCard.gameObject);

            _leftOperand._cardInSlot.oldValue = _leftOperand._cardInSlot.Value = value;
            
            PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
            Debug.Log(value);
        }

        public void AttackEnemy()
        {
            var attackCardNumber = _leftOperand._cardInSlot.Value;
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
                    _leftOperand._cardInSlot = null;
                    Destroy(leftCard.gameObject);
                    
                    PlayerHandComponent.Instance.HandPush(DeckComponent.Instance.DeckPop());
                }
            }
        }
    }
}