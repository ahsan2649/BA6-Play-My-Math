using System;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;

namespace Programming.Operation_Board {
    public enum OperationType {
        Nop,
        Add,
        Subtract,
        Multiply,
        Divide,
        Simplify,
        Expand,
    }

    public class OperationBoard : IFractionable {
        private ICardable _activeCard;
        private ICardable _additionalCard;
        private OperationType _currentOperation = OperationType.Nop;
        private int _modificationAmount;

        public event Action UpdateVisual;

        public void PushCard(ICardable newCard, OperationType operation = OperationType.Nop)
        {
            if (_activeCard == null)
            {
                SetCard(ref _activeCard, newCard, operation);
                return;
            }

            if (_additionalCard == null  && _currentOperation == OperationType.Nop)
            {
                SetCard(ref _additionalCard, newCard, operation switch
                {
                    OperationType.Nop => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                    OperationType.Add => operation,
                    OperationType.Subtract => operation,
                    OperationType.Multiply => operation,
                    OperationType.Divide => operation,
                    OperationType.Simplify => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                    OperationType.Expand => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                    _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
                });
            }
        }

        public void ModifyCard(OperationType operation, int modificationAmount)
        {
            if (_additionalCard != null)
            {
                throw new InvalidOperationException("Expand/Simplify only works on a single card");
            }

            _currentOperation = operation switch
            {
                OperationType.Nop => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                OperationType.Add => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                OperationType.Subtract => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                OperationType.Multiply => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                OperationType.Divide => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
                OperationType.Simplify => operation,
                OperationType.Expand => operation,
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
            _modificationAmount = modificationAmount;
            UpdateVisual?.Invoke();
        }

        public ICardable PopCard()
        {
            if (_additionalCard != null)
            {
                _currentOperation = OperationType.Nop;
                return RemoveCard(ref _additionalCard);
            }

            _currentOperation = OperationType.Nop;
            return RemoveCard(ref _activeCard);
        }

        public ICardable FinalizeCard()
        {
            Card returningCard = new Card(GetValue());
            _additionalCard = null;
            _activeCard = null;
            _currentOperation = OperationType.Nop;
            return returningCard;
        }

        private void SetCard(ref ICardable targetCard, ICardable newCard, OperationType operation)
        {
            targetCard = newCard;
            _currentOperation = operation;
            UpdateVisual?.Invoke();
        }

        private ICardable RemoveCard(ref ICardable targetCard)
        {
            var returningCard = targetCard;
            targetCard = null;
            UpdateVisual?.Invoke();
            return returningCard;
        }

        public Fraction GetValue()
        {
            if (_additionalCard != null)
            {
                return _currentOperation switch
                {
                    OperationType.Nop => throw new ArgumentOutOfRangeException(),
                    OperationType.Add => _activeCard.GetValue() + _additionalCard.GetValue(),
                    OperationType.Subtract => _activeCard.GetValue() - _additionalCard.GetValue(),
                    OperationType.Multiply => _activeCard.GetValue() * _additionalCard.GetValue(),
                    OperationType.Divide => _activeCard.GetValue() / _additionalCard.GetValue(),
                    OperationType.Simplify => throw new ArgumentOutOfRangeException(),
                    OperationType.Expand => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (_activeCard != null)
            {
                return _currentOperation switch
                {
                    OperationType.Nop => _activeCard.GetValue(),
                    OperationType.Add => throw new ArgumentOutOfRangeException(),
                    OperationType.Subtract => throw new ArgumentOutOfRangeException(),
                    OperationType.Multiply => throw new ArgumentOutOfRangeException(),
                    OperationType.Divide => throw new ArgumentOutOfRangeException(),
                    OperationType.Simplify => _activeCard.GetValue().SimplifyBy(_modificationAmount),
                    OperationType.Expand => _activeCard.GetValue().ExpandBy(_modificationAmount),
                    _ => throw new ArgumentOutOfRangeException()
                }; 
            }

            return new Fraction(0, 1);
        }
    }
}