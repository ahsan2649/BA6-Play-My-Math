using System;
using System.Runtime.CompilerServices;
using Programming.Fraction_Engine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Programming.TeacherMode
{
    public abstract class FractionChanger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] protected GameObject numeratorDenominatorCanvas;
        private TMP_Text activeText;

        public virtual void OnPointerClick(PointerEventData eventData) //opens the general Canvas in which Numerator and Denominator are visible
        {
            if (!numeratorDenominatorCanvas.activeSelf)
            {
                numeratorDenominatorCanvas.SetActive(true);
            }
            else
            {
                numeratorDenominatorCanvas.SetActive(false);
                if (NumberInputArea.Instance.gameObject.activeSelf) {CloseInputArea();}
            }
        }

        protected void Start()
        {
            CheckEnable();
        }

        public abstract void CheckEnable(); 
        

        public void OpenInputArea(TMP_Text text) //opens the 0-9 digits input area
        {
            SetActiveTextField(text);
            NumberInputArea.Instance.OpenInputArea(this, text);
        }

        public void CloseInputArea()
        {
            SetActiveTextField(null);
            NumberInputArea.Instance.CloseInputArea();
        }
        
        private void SetActiveTextField(TMP_Text textField)
        {
            activeText = textField; 
        }
        
        public void SetText(int number)
        {
            activeText.text = number.ToString(); 
        }

        public void InputDone() => FinaliseInput(); 
        public abstract void FinaliseInput(); 
        
        public abstract Fraction GetChangeableFraction(); 
    }
}
