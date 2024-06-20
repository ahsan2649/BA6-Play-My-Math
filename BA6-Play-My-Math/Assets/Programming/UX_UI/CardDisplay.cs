using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Programming.Card_Mechanism;

/* Base class for displaying Cards in the Game
 */

namespace Programming.UX_UI
{
    public abstract class CardDisplay : MonoBehaviour
    {
        [SerializeField] GameObject highlight; 

        public ICardable cardBaseObject; 

        public abstract void UpdateValue(); 
        public abstract void UpdateVisual();
        public abstract string GetName(); 
        
        public void SwitchHighlight()
        {
            highlight.SetActive(!highlight.activeSelf);
        }
    }
}