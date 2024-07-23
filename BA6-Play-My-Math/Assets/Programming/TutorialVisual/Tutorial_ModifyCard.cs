using System.Collections;
using System.Collections.Generic;
using Programming.Card_Mechanism;
using Programming.Fraction_Engine;
using Programming.Operation_Board;
using UnityEngine;

namespace Programming.TutorialVisual
{
    public class Tutorial_ModifyCard : MonoBehaviour //Spawn or Activate this script in the Level about Expand/Simplify
    {
        [Header("References")] 
        [SerializeField] private OperationBoardComponent opBoard;
        [SerializeField] private GameObject dragIntoSlotImage; 
        [SerializeField] private GameObject clickOnCardImage;

        private List<NumberCardComponent> unchangedFractionCards = new List<NumberCardComponent>();
        private List<NumberCardComponent> changedFractionCards = new List<NumberCardComponent>(); 
        private int _timesValueChanged = 0;
        
        public void Start()
        {
            SetImagesInactive(); 
            
            foreach (HandSlotComponent slot in PlayerHandComponent.Instance.cardSlots)
            {
                slot.onCardChanged.AddListener(CheckOpenAndSubscription); //Opens if a fraction was made
            } 
            opBoard = opBoard ? opBoard : OperationBoardComponent.Instance;
            opBoard.onOperationBoardChange.AddListener(SetImageBasedOnLeftSlot);
            
            StartCoroutine(ChangeFirstHandCard()); 
        }

        private IEnumerator ChangeFirstHandCard()
        {
            yield return null;
            NumberCardComponent nC = PlayerHandComponent.Instance.cardSlots[0].GetNumberCard(); 
            nC.IsFraction = true; 
            nC.oldValue = nC.Value = new Fraction(2, 4); //ZyKa this does not properly change the Value of the card
        }

        
        private void OnDestroy()
        {
            foreach (HandSlotComponent slot in PlayerHandComponent.Instance.cardSlots)
            {
                slot.onCardChanged.RemoveListener(CheckOpenAndSubscription); 
            } 
            opBoard.onOperationBoardChange.RemoveListener(SetImageBasedOnLeftSlot);
        }

        public void CheckOpenAndSubscription()
        {
            NumberCardComponent numberCardComponent; 
            foreach (HandSlotComponent slot in PlayerHandComponent.Instance.cardSlots)
            {
                if (slot.HasCard() && 
                    (numberCardComponent = slot.GetNumberCard()) is not null &&
                    numberCardComponent.IsFraction &&
                    !unchangedFractionCards.Contains(numberCardComponent) && 
                    !changedFractionCards.Contains(numberCardComponent)
                    )
                {
                    unchangedFractionCards.Add(numberCardComponent);
                    numberCardComponent.onValueChange.AddListener(Close);
                    SetImageToDrag();
                }
            }
        }
        
        public void Close(NumberCardComponent numberCard)
        {
            if (numberCard is null)
            {
                return; 
            }
            
            _timesValueChanged++;
            numberCard.onValueChange.RemoveListener(Close);
            unchangedFractionCards.Remove(numberCard);
            changedFractionCards.Add(numberCard);
            Debug.Log("CloseModifyTutorial");
            SetImagesInactive();

            if (_timesValueChanged >= 2) //for some reason this means 3?
            {
                Destroy(gameObject);
            }
        }

        private void SetImageBasedOnLeftSlot()
        {
            if (opBoard.LeftOperand.HasCard() && unchangedFractionCards.Contains(opBoard.LeftOperand.GetNumberCard()))
            {
                SetImageToClick();
            }
            else
            {
                unchangedFractionCards.RemoveAll(number => number is null); 
                
                SetImagesInactive();
            }
        }

        private void SetImagesInactive()
        {
            dragIntoSlotImage.SetActive(false);
            clickOnCardImage.SetActive(false);
        }
        
        private void SetImageToDrag()
        {
            dragIntoSlotImage.SetActive(true);
            clickOnCardImage.SetActive(false);
        }

        private void SetImageToClick()
        {
            dragIntoSlotImage.SetActive(false);
            clickOnCardImage.SetActive(true);
        }
    }
}
