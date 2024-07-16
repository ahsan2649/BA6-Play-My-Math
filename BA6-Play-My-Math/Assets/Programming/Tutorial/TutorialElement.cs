using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Programming.Tutorial
{
    public abstract class TutorialElement : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialVisual;
        [FormerlySerializedAs("onStepFinished")] public UnityEvent onTutorialStepFinished;
        
        public void Awake()
        {
            ReactivateEvent();
            if (tutorialVisual is null)
            {
                tutorialVisual = this.gameObject; 
            }
        }

        public void ReactivateEvent()
        {
            GetActivationEvents().ForEach(e => e.AddListener(OpenTutorial));
            GetFinishEvents().ForEach(e => e.AddListener(CloseTutorial));
        }
        
        private void OpenTutorial()
        {
            if (CheckActivationCondition())
            {
                tutorialVisual.SetActive(true);            
                GetActivationEvents().ForEach(e => e.RemoveListener(OpenTutorial));
            }
        }
        
        private void CloseTutorial()
        {
            if (CheckFinishCondition() && tutorialVisual.activeSelf)
            {
                tutorialVisual.SetActive(false);
                onTutorialStepFinished.Invoke();
                GetFinishEvents().ForEach(e => e.RemoveListener(CloseTutorial));
            }
        }

        protected virtual List<UnityEvent> GetActivationEvents()
        {
            return new List<UnityEvent>(); 
        }

        protected virtual List<UnityEvent> GetFinishEvents()
        {
            return new List<UnityEvent>(); 
        }
        
        protected virtual bool CheckFinishCondition()
        {
            return true; 
        }

        protected virtual bool CheckActivationCondition()
        {
            return true; 
        }
    }
}
