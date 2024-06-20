using System;
using Programming.Card_Mechanism;
using Programming.Enemy;
using Programming.Fraction_Engine;

namespace Programming.Operation_Board {
    public enum OperationType {
        Nop,
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    public class OperationBoard : IFractionable {
        public IFractionableCard LeftOperand;
        public IFractionableCard RightOperand;
        private OperationType _currentOperation = OperationType.Nop;
        
        public event Action UpdateVisual;

        public void PushCard(IFractionableCard newCard)
        {
            if (LeftOperand == null)
            {
                SetCard(ref LeftOperand, newCard);
                return;
            }

            if (RightOperand == null)
            {
                SetCard(ref RightOperand, newCard);
                return;
            }

            throw new InvalidOperationException("Both operands are occupied!");
        }
        
        public void SetCard(ref IFractionableCard targetOperand, in IFractionableCard newCard)
        {
            targetOperand = newCard;
            if (RightOperand != null
                && LeftOperand != null
                && _currentOperation == OperationType.Nop)
            {
                SetOperation(OperationType.Add);
            }

            UpdateVisual?.Invoke();
        }

        public void SwapCards(ref IFractionableCard targetOperand, ref IFractionableCard substituteCard)
        {
            (targetOperand, substituteCard) = (substituteCard, targetOperand);
            UpdateVisual?.Invoke();
        }

        public IFractionableCard RemoveCard(ref IFractionableCard targetOperand)
        {
            var returningCard = targetOperand;
            targetOperand = null;
            _currentOperation = OperationType.Nop;
            UpdateVisual?.Invoke();
            return returningCard;
        }

        public void SetOperation(OperationType operation)
        {
            if (LeftOperand == null || RightOperand == null)
            {
                throw new InvalidOperationException("Can't operate without two operands\\(Fractions\\)!");
            }
            _currentOperation = operation;
            UpdateVisual?.Invoke();
        }

        public IFractionableCard FinalizeCard()
        {
            if (LeftOperand != null && RightOperand != null)
            {
                return new NumberCard(GetValue());
            }

            throw new InvalidOperationException();
        }

        public void Attack(EnemyQueue enemyQueue, ref Enemy.Enemy enemy)
        {
            if (enemyQueue.RemoveEnemy(ref enemy) != null)
            {
                RemoveCard(ref LeftOperand); 
                //TODO: Draw new Card (e.g. add the draw on the same UI-Button or do a GetComponent)
            }
        }

        public Fraction GetValue()
        {
            if (LeftOperand != null && RightOperand != null)
            {
                return _currentOperation switch
                {
                    OperationType.Nop => throw new ArgumentOutOfRangeException(),
                    OperationType.Add => LeftOperand.GetValue() + RightOperand.GetValue(),
                    OperationType.Subtract => LeftOperand.GetValue() - RightOperand.GetValue(),
                    OperationType.Multiply => LeftOperand.GetValue() * RightOperand.GetValue(),
                    OperationType.Divide => LeftOperand.GetValue() / RightOperand.GetValue(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (LeftOperand != null)
            {
                return LeftOperand.GetValue();
            }

            if (RightOperand != null)
            {
                return RightOperand.GetValue();
            }

            return new Fraction(0, 1);
        }
    }
}