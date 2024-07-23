using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Programming.Operation_Board {
    public class FinalizeButtonComponent : MonoBehaviour, IPointerClickHandler
    {
        public OperationBoardComponent operationBoardComponent; 
        
        Canvas _canvas; 
        
        [SerializeField] private Animator animator; 
        private static readonly int Spin = Animator.StringToHash("Spin");
        
        public UnityEvent FinalizeEvent;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            
            _canvas.worldCamera = Camera.main;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (operationBoardComponent.CalculateCombinedValue() is not null)
            {
                FinalizeEvent.Invoke();
            }
            else
            {
                animator.SetTrigger(Spin);
            }
        }

        public void SetActiveAndEnabled(bool bActive)
        {
            transform.parent.gameObject.SetActive(bActive);
            enabled = bActive; 
        }
    }
}
