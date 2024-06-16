using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;

enum OperationType
{
    Add,
    Subtract, 
    Multiply,
    Divide,
    Simplify,
    Expand,
}

public class OperationBoard : IFractionable
{
    private ICardable _activeCard;
    private ICardable _additionalCard;
    private OperationType _currentOperation;
    private int _modificationAmount;

    void SetCard(ICardable newCard, OperationType operation = default)
    {
        if (_activeCard == null)
        {
            _activeCard = newCard;
            return;
        }

        if (_currentOperation == default)
        {
            return;
        }

        if (_activeCard.GetValue().Denominator == newCard.GetValue().Denominator)
        {
            _currentOperation = operation;
            _additionalCard = newCard;
        }
    }

    void ModifyCard(OperationType operation, int amount)
    {
        switch (operation)
        {
            case OperationType.Add:
            case OperationType.Subtract:
            case OperationType.Multiply:
            case OperationType.Divide:
                throw new InvalidEnumArgumentException(
                    "Invalid Operation for Modification: Only Expand and Simplify are allowed"
                    );
            case OperationType.Simplify:
            case OperationType.Expand:
                _currentOperation = operation;
                _modificationAmount = amount;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }
        _currentOperation = operation;
        _modificationAmount = amount;
    }

    void RemoveCard()
    {
        if (_additionalCard != null)
        {
            _additionalCard = null;
            _currentOperation = default;
            return;
        }

        if (_activeCard != null)
        {
            _activeCard = null;
            return;
        }
    }

    Card FinalizeCard()
    {
        Card finalizedCard;
        switch (_currentOperation)
        {
            case OperationType.Add:
                finalizedCard = new Card(_activeCard.GetValue() + _additionalCard.GetValue());
                break;
            case OperationType.Subtract:
                finalizedCard = new Card(_activeCard.GetValue() - _additionalCard.GetValue());
                break;
            case OperationType.Multiply:
                finalizedCard = new Card(_activeCard.GetValue() * _additionalCard.GetValue());
                break;
            case OperationType.Divide:
                finalizedCard = new Card(_activeCard.GetValue() / _additionalCard.GetValue());
                break;
            case OperationType.Simplify:
                finalizedCard = new Card(_activeCard.GetValue().SimplifyBy(_modificationAmount));
                break;
            case OperationType.Expand:
                finalizedCard = new Card(_activeCard.GetValue().ExpandBy(_modificationAmount));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _activeCard = null;
        _additionalCard = null;
        _modificationAmount = 0;
        _currentOperation = default;
        return finalizedCard;
    }
    
    public override string ToString()
    {
        return
            $"ActiveCard: {_activeCard}, AdditionalCard: {_additionalCard}" +
            $", Operation: {_currentOperation}, Amount: {_modificationAmount}";
    }

    public Fraction GetValue()
    {
        throw new NotImplementedException();
    }

    public Fraction SetValue(Fraction newValue)
    {
        throw new NotImplementedException();
    }
}
