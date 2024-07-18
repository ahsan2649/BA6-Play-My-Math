using System;
using System.Collections.Generic;
using UnityEngine;

namespace Programming.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TutorialElement[] TutorialElements;
        private int activeElementIndex = 0;

        private void Awake()
        {
            RestartTutorial();
        }

        public void ActivateFirstElement()
        {
            TutorialElements[0].OpenTutorial();
            TutorialElements[0].onTutorialStepFinished.AddListener(ActivateNextElement);
        }
        
        public void ActivateNextElement()
        {
            TutorialElements[activeElementIndex].onTutorialStepFinished.RemoveListener(ActivateNextElement);
            
            int i = activeElementIndex; 
            while (i < TutorialElements.Length && TutorialElements[i].bFinished)
            {
                i++; 
            }

            if (i > TutorialElements.Length)
            {
                Debug.Log("Tutorial Finished");
                return; 
            }
            
            activeElementIndex = i; 
            TutorialElements[activeElementIndex].OpenTutorial();
            TutorialElements[activeElementIndex].onTutorialStepFinished.AddListener(ActivateNextElement);
        }

        public void RestartTutorial()
        {
            for (int i = 0; i < TutorialElements.Length; i++)
            {
                TutorialElements[i].ResetEvents();
            }
            ActivateFirstElement();
        }
    }
}
