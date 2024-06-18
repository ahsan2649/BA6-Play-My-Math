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
    }

    public class OperationBoard : IFractionable {
        private ICardable _activeCard;
        private ICardable _additionalCard;
        private OperationType _currentOperation = OperationType.Nop;

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
                    _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
                });
            }
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
                    _ => throw new ArgumentOutOfRangeException()
                }; 
            }

            return new Fraction(0, 1);
        }
    }
}