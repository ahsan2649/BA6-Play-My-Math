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
        public bool bCanFinishBeforeActivate;
        public bool bFinished; 
        
        public void Awake()
        {
            ResetEvents();
            if (tutorialVisual is null)
            {
                tutorialVisual = this.gameObject; 
            }
        }

        public void ResetEvents()
        {
            GetActivationEvents().ForEach(e => e.AddListener(() => OpenTutorial()));
            GetFinishEvents().ForEach(e => e.AddListener(CloseTutorial));
            bFinished = false; 
        }
        
        public bool OpenTutorial()
        {
            if (CheckActivationCondition() && ! bFinished)
            {
                tutorialVisual.SetActive(true);            
                GetActivationEvents().ForEach(e => e.RemoveListener(() => OpenTutorial()));
                return true; 
            }
            else
            {
                return false; 
            }
        }
        
        public void CloseTutorial()
        {
            if (CheckFinishCondition() && (tutorialVisual.activeSelf || bCanFinishBeforeActivate))
            {
                tutorialVisual.SetActive(false);
                bFinished = true; 
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
