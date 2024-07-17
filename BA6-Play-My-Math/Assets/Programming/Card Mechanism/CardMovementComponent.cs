using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace Programming.Card_Mechanism {
    public class CardMovementComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        RectTransform _rectTransform;
        Canvas _canvas;
        CanvasGroup _canvasGroup;
        
        public SlotComponent currentSlot;
        
        [SerializeField] private float moveSpeed = 30f;
        [SerializeField] private float rotateSpeed = 250f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvas.worldCamera = Camera.main;
        }

        #region Drag

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                eventData.position, _canvas.worldCamera, out var pos);
            _rectTransform.position = _canvas.transform.TransformPoint(pos);
        }
        
        

       
        
        #endregion

        #region Animations
        
        public void TransformToNewParentCoroutines()
        {
            StartCoroutine(MoveToNewParent());
            StartCoroutine(RotateToNewParent()); 
            _canvasGroup.blocksRaycasts = true;
        }

        public IEnumerator MoveToNewParent()
        {
            while (Vector3.Distance(transform.position, transform.parent.position) > 0.01f)
            {
                _canvasGroup.blocksRaycasts = false;
                transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            _canvasGroup.blocksRaycasts = true;
        }

        public IEnumerator RotateToNewParent()
        {
            while (Quaternion.Angle(transform.rotation, transform.parent.rotation) > 0.1f)
            {
                _canvasGroup.blocksRaycasts = false;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, rotateSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            _canvasGroup.blocksRaycasts = true;
        }
        

        #endregion

        public void OnEndDrag(PointerEventData eventData)
        {
            TransformToNewParentCoroutines();
            _canvasGroup.blocksRaycasts = false;
        }
    }
}