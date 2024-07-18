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
            while (i < TutorialElements.Length)
            {
                if (TutorialElements[i].OpenTutorial())
                {
                    activeElementIndex = i; 
                    TutorialElements[activeElementIndex].onTutorialStepFinished.AddListener(ActivateNextElement);
                    break; 
                }
                
                i++; 
            }
            
            Debug.Log("Tutorial Finished");
        }

        public void RestartTutorial()
        {
            for (int i = 0; i < TutorialElements.Length; i++)
            {
                TutorialElements[i].ResetEvents();
            }
            ActivateFirstElement();
        }

        public void FinishTutorial()
        {
            for (int i = 0; i < TutorialElements.Length; i++)
            {
                TutorialElements[i].CloseTutorial();
            }
        }
    }
}
